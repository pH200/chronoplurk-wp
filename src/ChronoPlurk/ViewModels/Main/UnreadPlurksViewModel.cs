using System;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Plurto.Commands;
using Plurto.Core;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Main
{
    public sealed class UnreadPlurksViewModel : TimelineBaseViewModel<TimelineResult>, IRefreshSync
    {
        private IDisposable _getUnreadCountDisposable;

        public bool RefreshOnActivate { get; set; }

        public UnreadPlurksViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService)
            : base(navigationService, progressService, plurkService, plurkContentStorageService)
        {
            this.DisplayName = AppResources.filterUnread;
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
            if (_getUnreadCountDisposable != null)
            {
                _getUnreadCountDisposable.Dispose();
            }
            this.DisplayName = AppResources.filterUnread;
            var getUnreadCount = PollingCommand.GetUnreadCount().Client(PlurkService.Client).ToObservable();
            _getUnreadCountDisposable = getUnreadCount.ObserveOnDispatcher().Subscribe(count =>
            {
                this.DisplayName = AppResources.filterUnread + string.Format("({0})", count.All);
            });

            var getPlurks = TimelineCommand.GetUnreadPlurks().Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks));
                return TimelineCommand.GetUnreadPlurks(oldestOffset).Client(PlurkService.Client).ToObservable();
            };
            Request(getPlurks);
        }
    }
}
