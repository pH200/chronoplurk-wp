using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using Caliburn.Micro;
using MetroPlurk.Helpers;
using MetroPlurk.ViewModels;
using Plurto;
using Plurto.Commands;

namespace MetroPlurk.Services
{
    public interface IPlurkService
    {
        int UserId { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        CookieCollection Cookie { get; set; }
        IEnumerable<int> FriendsId { get; set; }
        bool IsLoaded { get; }
        IObservable<bool> LoginAsnc();
        IObservable<bool> LoginAsnc(string username, string password);
        void SaveUserData();
        bool LoadUserData();
        void ClearUserData();
    }

    public class PlurkService : IPlurkService
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CookieCollection Cookie { get; set; }
        public IEnumerable<int> FriendsId { get; set; }

        public bool IsLoaded { get; private set; }

        public IObservable<bool> LoginAsnc()
        {
            // Special handling for email login.
            var encodedUsername = HttpUtility.UrlEncode(Username);
            var login = UsersCommand.LoginNoData(encodedUsername, Password).LoadAsync();
            return login.Do(cookie =>
            {
                Cookie = cookie;
                IsLoaded = true;
            }).Select(c => c != null);
        }

        public IObservable<bool> LoginAsnc(string username, string password)
        {
            Username = username;
            Password = password;

            return LoginAsnc();
        }

        public void SaveUserData()
        {
            IsoSettings.AddOrChange(AppUserInfo.StorageKey,
                new AppUserInfo() { Username = Username, Password = Password });
        }

        public bool LoadUserData()
        {
            AppUserInfo user;
            if(IsoSettings.TryGetValue(AppUserInfo.StorageKey, out user))
            {
                Username = user.Username;
                Password = user.Password;
                return true;
            }
            return false;
        }

        public void ClearUserData()
        {
            Username = "";
            Password = "";
            Cookie = null;
            IsLoaded = false;
            IsoSettings.ClearAll();
        }
    }

    public class AppUserInfo
    {
        public const string StorageKey = "PlurkUser";
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
