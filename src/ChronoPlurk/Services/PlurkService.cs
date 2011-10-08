﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.ViewModels;
using Plurto;
using Plurto.Commands;
using Plurto.Core;
using Polenter.Serialization;

namespace ChronoPlurk.Services
{
    public interface IPlurkService
    {
        IRequestClient Client { get; }
        string Username { get; }
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

        public IEnumerable<int> FriendsId { get; set; }

        public bool IsLoaded
        {
            get { return (_appUserInfo != null && _appUserInfo.Cookie != null); }
        }

        public PlurkService(IProgressService progressService)
        {
            _progressService = progressService;
            _client = new LegacyClient(DefaultConfiguration.ApiKey);
            LoadUserData();
        }

        public IObservable<bool> LoginAsnc(string username, string password)
        {
            var login = UsersCommand.LoginNoData(username, password).Client(Client).ToObservable();
            return login.Do(cookie =>
            {
                _client.Cookies = cookie;
                _appUserInfo = new AppUserInfo()
                {
                    Username = username,
                    Password = password,
                    Cookie = cookie.OfType<Cookie>().FirstOrDefault(),
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
                SetCookie(_client, _appUserInfo.Cookie);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetCookie(LegacyClient client, Cookie cookie)
        {
            client.Cookies = new CookieCollection() {cookie};
        }

        public void ClearUserData()
        {
            IsoSettings.ClearAll();
        }
    }

    public sealed class AppUserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Cookie Cookie { get; set; }
    }
}