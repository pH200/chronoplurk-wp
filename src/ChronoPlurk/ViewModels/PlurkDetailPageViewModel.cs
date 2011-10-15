using System;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.Views;
using Plurto.Commands;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    public class PlurkDetailPageViewModel : LoginAvailablePage, INavigationInjectionRedirect
    {
        private IDisposable _favorite;
        private IDisposable _unfavorite;

        private IPlurkService PlurkService { get; set; }

        private PlurkDetailHeaderViewModel PlurkHeaderViewModel { get { return PlurkDetailViewModel.DetailHeader; } }
        
        public PlurkDetailViewModel PlurkDetailViewModel { get; private set; }
        
        public PlurkDetailPageViewModel
            (IPlurkService plurkService,
            PlurkDetailViewModel plurkDetailViewModel,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            PlurkService = plurkService;
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
            if (String.IsNullOrWhiteSpace(PlurkDetailViewModel.DetailFooter.PostContent))
            {
                PlurkDetailViewModel.ScrollToEnd();
            }
            else
            {
                PlurkDetailViewModel.DetailFooter.Reply();
            }
        }

        public void LikeAppBar()
        {
            var isLike = PlurkHeaderViewModel.IsFavorite;
            if (isLike)
            {
                if (_unfavorite != null)
                {
                    _unfavorite.Dispose();
                }
                _unfavorite = TimelineCommand.UnfavoritePlurks(PlurkHeaderViewModel.Id).Client(PlurkService.Client).ToObservable().Subscribe();
            }
            else
            {
                if (_favorite != null)
                {
                    _favorite.Dispose();
                }
                _favorite = TimelineCommand.FavoritePlurks(PlurkHeaderViewModel.Id).Client(PlurkService.Client).ToObservable().Subscribe();
            }
            PlurkHeaderViewModel.IsFavorite = !isLike;

            PlurkDetailViewModel.ScrollToTop();
            ReloadAppBar();
        }

        public void MuteAppBar()
        {
            var isMute = PlurkHeaderViewModel.IsUnread == UnreadStatus.Muted;
            if (isMute)
            {
                TimelineCommand.UnmutePlurks(PlurkHeaderViewModel.Id).Client(PlurkService.Client).ToObservable().Subscribe();
            }
            else
            {
                TimelineCommand.MutePlurks(PlurkHeaderViewModel.Id).Client(PlurkService.Client).ToObservable().Subscribe();
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
            return PlurkService.IsLoaded;
        }
        #endregion

        public object GetRedirectedViewModel()
        {
            return PlurkDetailViewModel.ListHeader;
        }
    }
}
