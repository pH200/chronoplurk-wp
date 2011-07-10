using System;
using System.Linq;
using Caliburn.Micro;
using MetroPlurk.Services;
using Plurto.Commands;
using Plurto.Entities;

namespace MetroPlurk.ViewModels
{
    public sealed class TimelineViewModel : TimelineBaseViewModel<PollingResult>, IRefreshSync
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
            var getPlurks =
                PollingCommand.GetPlurks(DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0)), 50)
                    .Client(PlurkService.Client).LoadAsync();
            RequestMoreHandler = plurks =>
                                 TimelineCommand.GetPlurks(new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks)), 50).
                                     Client(PlurkService.Client).LoadAsync();
            Request(getPlurks);
        }
    }
}
