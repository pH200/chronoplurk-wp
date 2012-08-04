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
    public sealed class MyPlurksViewModel : TimelineBaseViewModel<TimelineResult>, IRefreshSync
    {
        public bool RefreshOnActivate { get; set; }

        public MyPlurksViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService)
            : base(navigationService, progressService, plurkService, plurkContentStorageService)
        {
            this.DisplayName = AppResources.filterMy;
            this.CachingId = "my";
            IsHasMoreHandler = plurks =>
            {
                return plurks.Plurks != null
                       && plurks.Plurks.Count > 0;
            };
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks), DateTimeKind.Utc);
                return TimelineCommand.GetPlurks(oldestOffset,
                                                 filter: PlurksFilter.OnlyUser,
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

        private IObservable<TimelineResult> RequestHandler()
        {
            return TimelineCommand.GetPlurks(filter: PlurksFilter.OnlyUser,
                                             limit: DefaultConfiguration.RequestItemsLimit)
                .Client(PlurkService.Client).ToObservable();
        }

        public void RefreshSync()
        {
            Request(RequestHandler());
        }
    }
}
