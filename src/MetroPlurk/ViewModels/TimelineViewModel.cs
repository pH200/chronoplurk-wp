using System;
using System.Linq;
using Caliburn.Micro;
using MetroPlurk.Services;
using Plurto.Commands;
using Plurto.Entities;

namespace MetroPlurk.ViewModels
{
    public class TimelineViewModel : TimelineBaseViewModel<PollingResult>, IRefreshSync
    {
        public bool RefreshOnActivate { get; set; }

        public TimelineViewModel(IProgressService progressService, IPlurkService plurkService)
            : base(progressService, plurkService)
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
                PollingCommand.GetPlurks(
                PlurkService.Cookie, DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0)), 50).
                LoadAsync();
            RequestMoreHandler = plurks =>
                TimelineCommand.GetPlurks(
                PlurkService.Cookie, new DateTime(plurks.Plurks.Min(p => p.PostDate.Ticks)),50).
                LoadAsync();
            Request(getPlurks);
        }
    }
}
