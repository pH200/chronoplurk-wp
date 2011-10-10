using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Helpers;
using Plurto.Commands;
using Plurto.Core;

namespace ChronoPlurk.Services
{
    public interface IPlurkService
    {
        IRequestClient Client { get; }
        string Username { get; }
        int UserId { get; }
        IEnumerable<int> FriendsId { get; set; }
        bool IsLoaded { get; }
        IObservable<bool> LoginAsnc(string username, string password);
        void SaveUserData();
        bool LoadUserData();
        void ClearUserData();
    }

    public class PlurkService : IPlurkService
    {
        private readonly IProgressService _progressService;

        private readonly LegacyClient _client;

        private AppUserInfo _appUserInfo;

        private IDisposable _saveUserInfoDisposable;

        public IRequestClient Client { get { return _client; } }

        public string Username 
        {
            get { return (_appUserInfo == null ? null : _appUserInfo.Username); }
        }

        public int UserId
        {
            get { return (_appUserInfo == null ? -1 : _appUserInfo.UserId); }
        }

        public IEnumerable<int> FriendsId { get; set; }

        public bool IsLoaded
        {
            get { return (_appUserInfo != null && _appUserInfo.IsHasCookies); }
        }

        public PlurkService(IProgressService progressService)
        {
            _progressService = progressService;
            _client = new LegacyClient(DefaultConfiguration.ApiKey);
            LoadUserData();
        }

        public IObservable<bool> LoginAsnc(string username, string password)
        {
            var login = UsersCommand.Login(username, password).Client(Client).ToObservable();
            return login.Do(profile =>
            {
                var cookies = profile.Cookies;
                _client.Cookies = cookies;
                _appUserInfo = new AppUserInfo()
                {
                    Username = username,
                    Password = password,
                    Cookies = cookies.OfType<Cookie>().ToArray(),
                    UserId = profile.UserInfo.Id,
                };
            }).Select(c => c != null);
        }

        public void SaveUserData()
        {
            if (_appUserInfo != null)
            {
                _progressService.Show("Creating Profile");
                if (_saveUserInfoDisposable != null)
                {
                    _saveUserInfoDisposable.Dispose();
                }
                _saveUserInfoDisposable =
                    Observable.Start(() => IsoSettings.SerializeStore(_appUserInfo, "appUserInfo.bin"))
                        .Subscribe(unit => { },
                                   () => Execute.OnUIThread(() => _progressService.Hide()));
            }
        }

        public bool LoadUserData()
        {
            var appUserInfo = IsoSettings.DeserializeLoad("appUserInfo.bin") as AppUserInfo;
            if (appUserInfo != null)
            {
                _appUserInfo = appUserInfo;
                SetCookie(_client, _appUserInfo.Cookies);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void SetCookie(LegacyClient client, IEnumerable<Cookie> cookies)
        {
            var cookieCollection = new CookieCollection();
            foreach (var cookie in cookies)
            {
                cookieCollection.Add(cookie);
            }
            client.Cookies = cookieCollection;
        }

        public void ClearUserData()
        {
            IsoSettings.ClearAll();
        }
    }
}
