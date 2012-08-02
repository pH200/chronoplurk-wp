using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Plurto.Commands;
using Plurto.Core;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Main
{
    public sealed class TimelineViewModel : TimelineBaseViewModel<TimelineResult>, IRefreshSync, IChildT<PlurkMainPageViewModel>
    {
        public bool RefreshOnActivate { get; set; }

        public TimelineViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService)
            : base(navigationService, progressService, plurkService, plurkContentStorageService)
        {
            this.DisplayName = AppResources.filterTimeline;
            this.CachingId = "all";
            LoadCachedItems();
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
            var getPlurks = TimelineCommand.GetPlurks().Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks), DateTimeKind.Utc);
                return TimelineCommand.GetPlurks(oldestOffset).Client(PlurkService.Client).ToObservable();
            };
            Request(getPlurks);
        }

        protected override void OnRequestCompleted(TimelineResult lastResult)
        {
            var unread = Items.Where(p => p.IsUnread == UnreadStatus.Unread).Take(10);
            var my = Items.Where(p => p.UserId == p.ClientUserId).Take(10);
            var pri = Items.Where(p => p.IsPrivateVisibilityView == Visibility.Visible).Take(10);
            var liked = Items.Where(p => p.IsFavorite || p.Replurked == true).Take(10);

            var parent = this.GetParent();
            if (parent != null)
            {
                parent.LoadPrecachedItems(unread, my, pri, liked);
            }

            base.OnRequestCompleted(lastResult);
        }
    }
}
