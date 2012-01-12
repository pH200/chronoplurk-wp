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
            var getPlurks = TimelineCommand.GetPlurks(filter:PlurksFilter.OnlyFavorite).Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks));
                return TimelineCommand.GetPlurks(oldestOffset, filter:PlurksFilter.OnlyFavorite).Client(PlurkService.Client).ToObservable();
            };
            Request(getPlurks);
        }
    }
}
