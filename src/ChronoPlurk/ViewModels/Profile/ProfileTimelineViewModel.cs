using System;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Services;
using Plurto.Commands;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Profile
{
    public class ProfileTimelineViewModel : TimelineBaseViewModel<TimelineResult>, IRefreshSync
    {
        public bool RefreshOnActivate { get; set; }

        public int UserId { get; set; }

        public ProfileTimelineViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService)
            : base(navigationService, progressService, plurkService, plurkContentStorageService)
        {
            this.DisplayName = "timeline";
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
            var getPlurks = TimelineCommand.GetPublicPlurks(UserId).Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks));
                return TimelineCommand.GetPublicPlurks(UserId, oldestOffset).Client(PlurkService.Client).ToObservable();
            };
            Request(getPlurks);
        }
    }
}
