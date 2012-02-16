﻿using System;
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

        public int UnreadCount { get; set; }

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

        public override void SetAsRead(long plurkId)
        {
            var item = Items.FirstOrDefault(p => plurkId == p.PlurkId);
            if (item != null)
            {
                item.IsUnread = UnreadStatus.Read;
                --UnreadCount;
                RefreshUnreadCount();
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
                UnreadCount = count.All;
                RefreshUnreadCount();
            });

            var getPlurks = TimelineCommand.GetUnreadPlurks().Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks), DateTimeKind.Utc);
                return TimelineCommand.GetUnreadPlurks(oldestOffset).Client(PlurkService.Client).ToObservable();
            };
            Request(getPlurks);
        }
    }
}
