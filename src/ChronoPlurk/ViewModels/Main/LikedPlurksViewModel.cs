using System;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Plurto.Commands;
using Plurto.Core;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Main
{
    public sealed class LikedPlurksViewModel : TimelineBaseViewModel<TimelineResult>, IRefreshSync
    {
        public bool RefreshOnActivate { get; set; }

        public LikedPlurksViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService)
            : base(navigationService, progressService, plurkService, plurkContentStorageService)
        {
            this.DisplayName = AppResources.filterLiked;
            this.CachingId = "liked";
            // LoadCachedItems();
            IsHasMoreHandler = plurks =>
            {
                return plurks.Plurks != null &&
                       plurks.Plurks.Count > 0;
            };
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks), DateTimeKind.Utc);
                return TimelineCommand.GetPlurks(oldestOffset,
                                                 filter: PlurksFilter.OnlyFavorite,
                                                 limit: DefaultConfiguration.RequestItemsLimit)
                    .Client(PlurkService.Client).ToObservable();
            };
            RequestMoreFromPrecachedHandler = items =>
            {
                if (!items.IsNullOrEmpty())
                {
                    var oldestOffset = new DateTime(items.Min(p => p.PostDate.Ticks), DateTimeKind.Utc);
                    return TimelineCommand.GetPlurks(oldestOffset, filter: PlurksFilter.OnlyFavorite).Client(PlurkService.Client).ToObservable();
                }
                else
                {
                    return RequestHandler();
                }
            };
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (RefreshOnActivate)
            {
                RefreshOnActivate = false;
                if (!IsFreshPrecachedItemsLoaded) RefreshSync();
            }
        }

        private IObservable<TimelineResult> RequestHandler()
        {
            return TimelineCommand.GetPlurks(filter: PlurksFilter.OnlyFavorite,
                                             limit: DefaultConfiguration.RequestItemsLimit)
                .Client(PlurkService.Client).ToObservable();
        }

        public void RefreshSync()
        {
            Request(RequestHandler());
        }
    }
}
