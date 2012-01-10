using System;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Services;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
{
    // Content may vary depending on user.
    [NotifyForAll]
    public sealed class PlurkMainPageViewModel : PlurkAppBarPage
    {
        private readonly TimelineViewModel _timeline;
        
        public string Username { get; set; }

        public string UserAvatar { get; set; }

        public bool NewPost { get; set; }

        public PlurkMainPageViewModel(
            INavigationService navigationService,
            IPlurkService plurkService,
            LoginViewModel loginViewModel,
            TimelineViewModel timeline)
            : base(navigationService, plurkService, loginViewModel)
        {
            _timeline = timeline;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.Add(_timeline);
            ActivateItem(_timeline);
        }

        protected override void OnActivate()
        {
            if (PlurkService.IsUserChanged)
            {
                PlurkService.IsUserChanged = false;
                var user = PlurkService.AppUserInfo;
                if (user != null)
                {
                    Username = PlurkService.AppUserInfo.Username;
                    UserAvatar = PlurkService.AppUserInfo.UserAvatar;
                }
                RefreshAll();
            }

            NavigationService.UseRemoveBackEntryFlag(); // Remove LoginPage entry.

            if (NewPost)
            {
                _timeline.RefreshOnActivate = true;
                ActivateItem(_timeline);
                NewPost = false;
            }

            base.OnActivate();
        }

        private void RefreshAll()
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

        #region AppBar
        
        public void RefreshAppBar()
        {
            RefreshAll();
        }

        #endregion
    }
}
