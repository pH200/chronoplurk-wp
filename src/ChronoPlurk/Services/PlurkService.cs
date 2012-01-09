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
        void Favorite(int id);
        void Unfavorite(int id);
        void Mute(int id);
        void Unmute(int id);
        void SetAsRead(int id);
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
            _appUserInfo = null;
            IsoSettings.ClearAll();
        }

        private void SimpleAction<T>(CommandBase<T> command, Action<PlurkHolderService> holderAction)
        {
            if (IsLoaded)
            {
                command.Client(Client).ToObservable().Subscribe();
                var service = IoC.Get<PlurkHolderService>();
                if (service != null)
                {
                    holderAction(service);
                }
            }
        }

        public void Favorite(int id)
        {
            SimpleAction(TimelineCommand.FavoritePlurks(id), service => service.Favorite(id));
        }

        public void Unfavorite(int id)
        {
            SimpleAction(TimelineCommand.UnfavoritePlurks(id), service => service.Unfavorite(id));
        }

        public void Mute(int id)
        {
            SimpleAction(TimelineCommand.MutePlurks(id), service => service.Mute(id));
        }

        public void Unmute(int id)
        {
            SimpleAction(TimelineCommand.UnmutePlurks(id), service => service.Unmute(id));
        }

        public void SetAsRead(int id)
        {
            SimpleAction(TimelineCommand.MarkAsRead(id), service => service.SetAsRead(id));
        }
    }
}
