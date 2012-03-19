using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Services;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels.Profile
{
    [NotifyForAll]
    public sealed class PlurkProfilePageViewModel : PlurkAppBarPage
    {
        private readonly ProfileTimelineViewModel _timeline;

        public int UserId { get; set; }

        public string Username { get; set; }

        public string UserAvatar { get; set; }

        public PlurkProfilePageViewModel(
            INavigationService navigationService,
            IPlurkService plurkService,
            ProfileTimelineViewModel timeline)
            : base(navigationService, plurkService)
        {
            _timeline = timeline;
        }

        protected override void OnInitialize()
        {
            Items.Add(_timeline);
            ActivateItem(_timeline);
            _timeline.UserId = UserId;
            _timeline.Username = Username;
            _timeline.RefreshSync();

            base.OnInitialize();
        }

        #region AppBar

        public void RefreshAppBar()
        {
            foreach (var screen in Items.OfType<IRefreshSync>())
            {
                if (screen == ActiveItem)
                {
                    screen.RefreshSync();
                }
                else
                {
                    screen.RefreshOnActivate = true;
                }
            }
        }

        #endregion
    }
}
