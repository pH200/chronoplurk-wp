using System;
using System.Linq;
using Caliburn.Micro;
using MetroPlurk.Services;
using MetroPlurk.Views;
using Plurto.Commands;
using Plurto.Core;

namespace MetroPlurk.ViewModels
{
    public class PlurkDetailPageViewModel : LoginAvailablePage, INavigationInjectionRedirect
    {
        private readonly IPlurkService _plurkService;
        
        public PlurkDetailViewModel PlurkDetailViewModel { get; private set; }

        private PlurkDetailHeaderViewModel PlurkHeaderViewModel { get { return PlurkDetailViewModel.ListHeader; } }
        
        public PlurkDetailPageViewModel
            (IPlurkService plurkService,
            PlurkDetailViewModel plurkDetailViewModel,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            _plurkService = plurkService;
            PlurkDetailViewModel = plurkDetailViewModel;
        }

        protected override void OnActivate()
        {
            PlurkDetailViewModel.RefreshOnActivate = true;
            ActivateItem(PlurkDetailViewModel);

            base.OnActivate();
        }

        protected override void OnViewLoaded(object view)
        {
            ReloadAppBar(view as PlurkDetailPage);

            base.OnViewLoaded(view);
        }

        #region AppBar

        private void ReloadAppBar(PlurkDetailPage view)
        {
            if (view == null)
            {
                return;
            }

            foreach (AppBarMenuItem menuItem in view.ApplicationBar.MenuItems)
            {
                if (menuItem.Message == "LikeAppBar")
                {
                    menuItem.Text = PlurkHeaderViewModel.LikeText;
                }
                if (menuItem.Message == "MuteAppBar")
                {
                    menuItem.Text = PlurkHeaderViewModel.MuteText;
                }
            }
        }

        private void ReloadAppBar()
        {
            ReloadAppBar(GetView(null) as PlurkDetailPage);
        }

        public void RefreshAppBar()
        {
            PlurkDetailViewModel.RefreshSync();
        }

        public void ReplyAppBar()
        {
            PlurkDetailViewModel.ScrollToEnd();
        }

        public void LikeAppBar()
        {
            var isLike = PlurkHeaderViewModel.IsFavorite;
            if (isLike)
            {
                TimelineCommand.UnfavoritePlurks(_plurkService.Cookie, PlurkHeaderViewModel.Id).LoadAsync().Subscribe();
            }
            else
            {
                TimelineCommand.FavoritePlurks(_plurkService.Cookie, PlurkHeaderViewModel.Id).LoadAsync().Subscribe();
            }
            PlurkHeaderViewModel.IsFavorite = !isLike;

            ReloadAppBar();
        }

        public void MuteAppBar()
        {
            var isMute = PlurkHeaderViewModel.IsUnread == UnreadStatus.Muted;
            if (isMute)
            {
                TimelineCommand.UnmutePlurks(_plurkService.Cookie, PlurkHeaderViewModel.Id).LoadAsync().Subscribe();
            }
            else
            {
                TimelineCommand.MutePlurks(_plurkService.Cookie, PlurkHeaderViewModel.Id).LoadAsync().Subscribe();
            }
            var unreadInt = (!isMute) ? (int)UnreadStatus.Muted : (int)UnreadStatus.Read;
            PlurkHeaderViewModel.IsUnreadInt = unreadInt;

            ReloadAppBar();
        }

        #endregion

        #region Can...
        public bool CanReplyAppBar()
        {
            return IsLoggedIn();
        }

        public bool CanLikeAppBar()
        {
            return IsLoggedIn();
        }

        public bool CanMuteAppBar()
        {
            return IsLoggedIn();
        }

        private bool IsLoggedIn()
        {
            return _plurkService.IsLoaded;
        }
        #endregion

        public object GetRedirectedViewModel()
        {
            return PlurkDetailViewModel.ListHeader;
        }
    }
}
