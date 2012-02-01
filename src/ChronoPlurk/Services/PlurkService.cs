using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using Plurto.Commands;
using Plurto.Core;

namespace ChronoPlurk.Services
{
    public interface IPlurkService
    {
        IRequestClient Client { get; }
        string Username { get; }
        int UserId { get; }
        AppUserInfo AppUserInfo { get; }
        IEnumerable<int> FriendsId { get; set; }
        bool IsLoaded { get; }
        bool IsUserChanged { get; set; }
        IObservable<bool> LoginAsnc(string username, string password);
        void ClearUserCookie();
        void SaveUserData();
        bool LoadUserData();
        void ClearUserData();
        void Favorite(long plurkId);
        void Unfavorite(long plurkId);
        void Mute(long plurkId);
        void Unmute(long plurkId);
        void SetAsRead(long plurkId);
        void Delete(long plurkId);
    }

    public class PlurkService : IPlurkService
    {
        private readonly IProgressService _progressService;

        private readonly LegacyClient _client;

        public AppUserInfo AppUserInfo { get; private set; }

        private IDisposable _saveUserInfoDisposable;

        public IRequestClient Client { get { return _client; } }

        public string Username 
        {
            get { return (AppUserInfo == null ? null : AppUserInfo.Username); }
        }

        public int UserId
        {
            get { return (AppUserInfo == null ? -1 : AppUserInfo.UserId); }
        }

        public IEnumerable<int> FriendsId { get; set; }

        public bool IsLoaded
        {
            get { return (AppUserInfo != null && AppUserInfo.IsHasCookies); }
        }

        public bool IsUserChanged { get; set; }

        public PlurkService(IProgressService progressService)
        {
            _progressService = progressService;

            // TODO: Change this when plurks caching implemented.
            IsUserChanged = true; // Reload on startup.

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
                if (Username != username)
                {
                    IsUserChanged = true;
                }
                AppUserInfo = new AppUserInfo()
                {
                    Username = username,
                    Password = password,
                    Cookies = cookies.OfType<Cookie>().ToArray(),
                    UserId = profile.UserInfo.Id,
                    UserAvatar = profile.UserInfo.AvatarBig,
                };
                foreach (var cookie in AppUserInfo.Cookies)
                {
                    cookie.Expires = cookie.Expires.AddYears(20);
                }
            }).Select(c => c != null);
        }

        public void ClearUserCookie()
        {
            if (AppUserInfo != null)
            {
                AppUserInfo.Cookies = null;
            }
        }

        public void SaveUserData()
        {
            if (AppUserInfo != null)
            {
                _progressService.Show(AppResources.msgCreatingProfile);
                if (_saveUserInfoDisposable != null)
                {
                    _saveUserInfoDisposable.Dispose();
                }
                _saveUserInfoDisposable =
                    Observable.Start(() => IsoSettings.SerializeStore(AppUserInfo, "appUserInfo.bin"))
                        .Subscribe(unit => { },
                                   () => Execute.OnUIThread(() => _progressService.Hide()));
            }
        }

        public bool LoadUserData()
        {
            var appUserInfo = IsoSettings.DeserializeLoad("appUserInfo.bin") as AppUserInfo;
            if (appUserInfo != null)
            {
                AppUserInfo = appUserInfo;
                SetCookie(_client, AppUserInfo.Cookies);
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
            AppUserInfo = null;
            IsoSettings.ClearAll();
        }

        private void SimpleAction<T>(CommandBase<T> command, Action<PlurkHolderService> holderAction)
        {
            if (IsLoaded)
            {
                var service = IoC.Get<PlurkHolderService>();
                if (service != null)
                {
                    holderAction(service);
                }
                command.Client(Client).ToObservable().PlurkException().Subscribe();
            }
        }

        public void Favorite(long plurkId)
        {
            SimpleAction(TimelineCommand.FavoritePlurks(plurkId), service => service.Favorite(plurkId));
        }

        public void Unfavorite(long plurkId)
        {
            SimpleAction(TimelineCommand.UnfavoritePlurks(plurkId), service => service.Unfavorite(plurkId));
        }

        public void Mute(long plurkId)
        {
            SimpleAction(TimelineCommand.MutePlurks(plurkId), service => service.Mute(plurkId));
        }

        public void Unmute(long plurkId)
        {
            SimpleAction(TimelineCommand.UnmutePlurks(plurkId), service => service.Unmute(plurkId));
        }

        public void SetAsRead(long plurkId)
        {
            SimpleAction(TimelineCommand.MarkAsRead(plurkId), service => service.SetAsRead(plurkId));
        }

        public void Delete(long plurkId)
        {
            SimpleAction(TimelineCommand.PlurkDelete(plurkId), service => { });
        }
    }
}
