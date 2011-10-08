using System;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Services;
using Plurto.Commands;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels
{
    public sealed class TimelineViewModel : TimelineBaseViewModel<TimelineResult>, IRefreshSync
    {
        public bool RefreshOnActivate { get; set; }

        public TimelineViewModel
            (INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService)
            : base(navigationService, progressService, plurkService)
        {
            this.DisplayName = "timeline";
            IsHasMoreHandler = plurks => { return plurks.Plurks != null && plurks.Plurks.Count > 0; };
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (!RefreshOnActivate) return;
            RefreshOnActivate = false;
            RefreshSync();
        }

        public void RefreshSync()
        {
            var getPlurks = TimelineCommand.GetPlurks().Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks =>
            {
                var oldestOffset = new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks));
                return TimelineCommand.GetPlurks(oldestOffset).Client(PlurkService.Client).ToObservable();
            };
            Request(getPlurks);
        }
    }
}
