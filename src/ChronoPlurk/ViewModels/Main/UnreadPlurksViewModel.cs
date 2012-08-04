using System;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
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

        public int UnreadCount { get; set; }

        public UnreadPlurksViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService)
            : base(navigationService, progressService, plurkService, plurkContentStorageService)
        {
            this.DisplayName = AppResources.filterUnread;
            this.CachingId = "unread";
            IsHasMoreHandler = plurks => { return plurks.Plurks != null && plurks.Plurks.Count > 0; };
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks), DateTimeKind.Utc);
                return TimelineCommand.GetUnreadPlurks(oldestOffset,
                                                       limit: DefaultConfiguration.RequestItemsLimit)
                    .Client(PlurkService.Client).ToObservable();
            };
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            
            if (RefreshOnActivate)
            {
                RefreshOnActivate = false;
                RefreshSync();
            }
        }

        public override void Mute(long plurkId)
        {
            UpdateItem(plurkId, UnreadStatus.Muted);
        }

        public override void SetAsRead(long plurkId)
        {
            UpdateItem(plurkId, UnreadStatus.Read);
        }

        private void UpdateItem(long plurkId, UnreadStatus status)
        {
            var item = Items.FirstOrDefault(p => plurkId == p.PlurkId);
            if (item != null)
            {
                if (item.IsUnread == UnreadStatus.Unread)
                {
                    --UnreadCount;
                    RefreshUnreadCount();
                }
                switch (status)
                {
                    case UnreadStatus.Muted:
                        item.IsUnread = status;
                        break;
                    case UnreadStatus.Read:
                        if (item.IsUnread == UnreadStatus.Unread)
                        {
                            item.IsUnread = UnreadStatus.Read;
                        }
                        break;
                }
            }
        }

        public void RemoveReadItems()
        {
            for (var i = Items.Count - 1; i >= 0; i--)
            {
                var item = Items[i];
                if (item.IsUnread != UnreadStatus.Unread)
                {
                    Items.RemoveAt(i);
                }
            }
            if (Items.Count == 0)
            {
                IsHasMore = false; // Looks better if cleared all items.
            }
        }

        public void RefreshUnreadCount()
        {
            if (UnreadCount > 0)
            {
                this.DisplayName = AppResources.filterUnread + "(" + UnreadCount + ")";
            }
            else
            {
                this.DisplayName = AppResources.filterUnread;
            }
        }

        /// <summary>
        /// Request new unread count from Plurk. Refresh automatically. Observes on dispatcher.
        /// </summary>
        public void RequestUnreadCount(Action<UnreadCount> onNext = null)
        {
            if (_getUnreadCountDisposable != null)
            {
                _getUnreadCountDisposable.Dispose();
            }
            var getUnreadCount = PollingCommand.GetUnreadCount()
                .Client(PlurkService.Client)
                .ToObservable()
                .Retry(DefaultConfiguration.RetryCount)
                .IgnoreAllExceptions()
                .ObserveOnDispatcher();
            _getUnreadCountDisposable = getUnreadCount.Subscribe(count =>
            {
                UnreadCount = count.All;
                RefreshUnreadCount();
                if (onNext != null)
                {
                    onNext(count);
                }
            });
        }

        private IObservable<TimelineResult> RequestHandler()
        {
            // Manually set limit, default sends all unreads.
            return TimelineCommand.GetUnreadPlurks(limit: 20).Client(PlurkService.Client).ToObservable();
        }

        public void RefreshSync()
        {
            RequestUnreadCount();
            
            Request(RequestHandler());
        }
    }
}
