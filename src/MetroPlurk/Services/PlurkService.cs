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
using Plurto.Core;

namespace MetroPlurk.Services
{
    public interface IPlurkService
    {
        IRequestClient Client { get; }
        int UserId { get; set; }
        string Username { get; set; }
        string Password { get; set; }
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
        private readonly LegacyClient _client;
        public IRequestClient Client { get { return _client; } }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public IEnumerable<int> FriendsId { get; set; }

        public bool IsLoaded { get; private set; }

        public PlurkService()
        {
            _client = new LegacyClient(DefaultConfiguration.ApiKey);
        }

        public IObservable<bool> LoginAsnc()
        {
            // Special handling for email login.
            var encodedUsername = HttpUtility.UrlEncode(Username);
            var login = UsersCommand.LoginNoData(encodedUsername, Password).Client(Client).LoadAsync();
            return login.Do(cookie =>
            {
                _client.Cookies = cookie;
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
