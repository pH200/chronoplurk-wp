﻿using System;
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
using Plurto.Core.OAuth;
using Plurto.Entities;

namespace ChronoPlurk.Services
{
    public interface IPlurkService
    {
        string VerifierTemp { get; set; }
        IRequestClient Client { get; }
        string Username { get; }
        int UserId { get; }
        AppUserInfo AppUserInfo { get; }
        bool IsLoaded { get; }
        bool IsUserChanged { get; set; }
        void FlushConnection();
        IObservable<Uri> GetRequestToken(string deviceId=null);
        IObservable<OAuthClient> GetAccessToken(string verifier);
        IObservable<AppUserInfo> CreateUserData(OAuthClient client);
        void SaveUserData();
        bool LoadUserData();
        void ClearUserData();

        void Favorite(long plurkId);
        void Unfavorite(long plurkId);
        void Mute(long plurkId);
        void Unmute(long plurkId);
        void SetAsRead(long plurkId);
        void MarkAsRead(IList<long> plurkIds);
        void Delete(long plurkId);
        void DeleteResponse(long id, long plurkId);
        void Replurk(long plurkId);
        void Unreplurk(long plurkId);
    }

    public class PlurkService : IPlurkService
    {
        private readonly IProgressService _progressService;

        private readonly OAuthClient _client;

        public AppUserInfo AppUserInfo { get; private set; }

        public string VerifierTemp { get; set; }

        public IRequestClient Client { get { return _client; } }

        public string Username
        {
            get { return (AppUserInfo == null ? null : AppUserInfo.Username); }
        }

        public int UserId
        {
            get { return (AppUserInfo == null ? -1 : AppUserInfo.UserId); }
        }

        public bool IsLoaded
        {
            get { return (AppUserInfo != null && AppUserInfo.HasAccessToken); }
        }

        public bool IsUserChanged { get; set; }

        public PlurkService(IProgressService progressService)
        {
            _progressService = progressService;

            // TODO: Change this when plurks caching implemented.
            IsUserChanged = true; // Reload on startup.

            _client = new OAuthClient(DefaultConfiguration.OAuthConsumerKey, DefaultConfiguration.OAuthConsumerSecret);
        }

        public IObservable<Uri> GetRequestToken(string deviceid=null)
        {
            string deviceIdPart;
            if (deviceid != null)
            {
                const string querystring = "&deviceid=";
                var purify = deviceid.Replace("&", "").Replace("?", "");
                // Also send querystring for escaping
                deviceIdPart = HttpTools.EscapeDataStringOmitNull(querystring + purify);
            }
            else
            {
                deviceIdPart = "";
            }
            return Observable.FromAsync(() => _client.ObtainRequestTokenAsync())
                .Do(token => _client.SetToken(token))
                .Select(token =>
                        new Uri("http://www.plurk.com/m/login?return_url=/m/authorize?oauth_token="
                                + token.Token
                                + deviceIdPart,
                                UriKind.Absolute));
        }

        public IObservable<OAuthClient> GetAccessToken(string verifier)
        {
            return Observable.FromAsync(() => _client.ObtainAccessTokenAsync(verifier))
                .Do(token => _client.SetToken(token)).Select(token => _client);
        }

        public void FlushConnection()
        {
            _client.FlushToken();
            if (AppUserInfo != null)
            {
                AppUserInfo.AccessToken = null;
                AppUserInfo.AccessTokenSecret = null;
            }
        }

        public IObservable<AppUserInfo> CreateUserData(OAuthClient client)
        {
            var profileObs = ProfileCommand.GetOwnProfile().Client(client).ToObservable();
            var userObs = UsersCommand.CurrentUser().Client(client).ToObservable();
            var zipObs = profileObs.Zip(userObs, (profile, user) => new { profile, user });
            var result = zipObs.Do(zip =>
            {
                AppUserInfo = new AppUserInfo()
                {
                    Username = zip.profile.UserInfo.NickName,
                    UserId = zip.profile.UserInfo.Id,
                    UserAvatar = zip.profile.UserInfo.AvatarBig,
                    User = zip.user,
                    AccessToken = _client.Token,
                    AccessTokenSecret = client.TokenSecret,
                };
                IsUserChanged = true;
            }).Select(zip => AppUserInfo);
            return result;
        }

        public void SaveUserData()
        {
            if (AppUserInfo != null)
            {
                Observable.Start(() => IsoSettings.SerializeStore(AppUserInfo, "appUserInfo.bin"))
                    .DoProgress(_progressService, AppResources.msgCreatingProfile)
                    .Subscribe();
            }
        }

        public bool LoadUserData()
        {
            var appUserInfo = IsoSettings.DeserializeLoad("appUserInfo.bin") as AppUserInfo;
            if (appUserInfo != null)
            {
                AppUserInfo = appUserInfo;
                _client.Token = appUserInfo.AccessToken;
                _client.TokenSecret = appUserInfo.AccessTokenSecret;
                return true;
            }
            else
            {
                return false;
            }
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
                command.Client(Client).ToObservable().PlurkException().SubscribeAndForget();
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

        public void MarkAsRead(IList<long> plurkIds)
        {
            SimpleAction(TimelineCommand.MarkAsRead(plurkIds), service => service.MarkAsRead(plurkIds));
        }

        public void Delete(long plurkId)
        {
            SimpleAction(TimelineCommand.PlurkDelete(plurkId), service => { });
        }

        public void DeleteResponse(long id, long plurkId)
        {
            SimpleAction(ResponsesCommand.ResponseDelete(id, plurkId), service => { });
        }

        public void Replurk(long plurkId)
        {
            SimpleAction(TimelineCommand.Replurk(plurkId), service => service.Replurk(plurkId));
        }

        public void Unreplurk(long plurkId)
        {
            SimpleAction(TimelineCommand.Unreplurk(plurkId), service => service.Unreplurk(plurkId));
        }
    }
}
