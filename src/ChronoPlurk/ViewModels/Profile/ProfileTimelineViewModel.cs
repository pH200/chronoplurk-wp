using System;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Plurto.Commands;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Profile
{
    public class ProfileTimelineViewModel : TimelineBaseViewModel<TimelineResult>, IRefreshSync, IChildT<PlurkProfilePageViewModel>
    {
        public bool RefreshOnActivate { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public ProfileTimelineViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService)
            : base(navigationService, progressService, plurkService, plurkContentStorageService)
        {
            this.DisplayName = AppResources.filterTimeline;
            IsHasMoreHandler = plurks => { return plurks.Plurks != null && plurks.Plurks.Count > 0; };
        }

        protected override void OnActivate()
        {
            if (RefreshOnActivate)
            {
                RefreshOnActivate = false;
                RefreshSync();
            }

            base.OnActivate();
        }

        public void RefreshSync()
        {
            if (UserId != 0)
            {
                SyncById();
            }
            else if (Username != null)
            {
                SyncByUsername();
            }
        }

        private void SyncById()
        {
            var getPlurks = TimelineCommand.GetPublicPlurks(UserId).Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks));
                return TimelineCommand.GetPublicPlurks(UserId, oldestOffset).Client(PlurkService.Client).ToObservable();
            };
            Request(getPlurks);
        }

        private void SyncByUsername()
        {
            var getPlurks = TimelineCommand.GetPublicPlurks(Username).Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks));
                return TimelineCommand.GetPublicPlurks(Username, oldestOffset).Client(PlurkService.Client).ToObservable();
            };
            Request(getPlurks);
        }

        protected override void OnRequestCompleted(TimelineResult lastResult)
        {
            var parent = this.GetParent();
            if (parent != null && parent.UserAvatar == null)
            {
                var user = lastResult.Users.Values.FirstOrDefault(u =>
                {
                    return u.Id == UserId ||
                           u.NickName == Username;
                });
                if (user != null)
                {
                    parent.UserAvatar = AvatarHelper.MapAvatar(user.AvatarBig);
                }
            }
        }
    }
}
