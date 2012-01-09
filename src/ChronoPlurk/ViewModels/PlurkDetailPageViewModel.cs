using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels.Core;
using ChronoPlurk.Views;
using Plurto.Commands;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    public class PlurkDetailPageViewModel : LoginAvailablePage, INavigationInjectionRedirect, IPlurkHolder
    {
        private IDisposable _favorite;
        private IDisposable _unfavorite;

        private IPlurkService PlurkService { get; set; }

        private PlurkDetailHeaderViewModel PlurkHeaderViewModel { get { return PlurkDetailViewModel.DetailHeader; } }

        protected PlurkHolderService PlurkHolderService { get; set; }
        
        public PlurkDetailViewModel PlurkDetailViewModel { get; private set; }
        
        public PlurkDetailPageViewModel
            (IPlurkService plurkService,
            PlurkHolderService plurkHolderService,
            PlurkDetailViewModel plurkDetailViewModel,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            PlurkHolderService = plurkHolderService;
            PlurkService = plurkService;
            PlurkDetailViewModel = plurkDetailViewModel;
        }

        protected override void OnActivate()
        {
            PlurkDetailViewModel.RefreshOnActivate = true;
            ActivateItem(PlurkHeaderViewModel);
            ActivateItem(PlurkDetailViewModel);
            // Read Plurk
            PlurkHolderService.SetAsRead(PlurkHeaderViewModel.Id);

            base.OnActivate();
        }

        protected override void OnViewLoaded(object view)
        {
            ReloadAppBar(view as PlurkDetailPage);
            PlurkHolderService.Add(this);

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
                PlurkHolderService.Unfavorite(PlurkHeaderViewModel.Id);
            }
            else
            {
                if (_favorite != null)
                {
                    _favorite.Dispose();
                }
                _favorite = TimelineCommand.FavoritePlurks(PlurkHeaderViewModel.Id).Client(PlurkService.Client).ToObservable().Subscribe();
                PlurkHolderService.Favorite(PlurkHeaderViewModel.Id);
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
                PlurkHolderService.Unmute(PlurkHeaderViewModel.Id);
            }
            else
            {
                TimelineCommand.MutePlurks(PlurkHeaderViewModel.Id).Client(PlurkService.Client).ToObservable().Subscribe();
                PlurkHolderService.Mute(PlurkHeaderViewModel.Id);
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

        #region IPlurkHolder
        
        // Ignore id.

        public IEnumerable<int> PlurkIds
        {
            get { return new[] { PlurkHeaderViewModel.Id }; }
        }

        public void Favorite(int id)
        {
            PlurkHeaderViewModel.IsFavorite = true;
        }

        public void Unfavorite(int id)
        {
            PlurkHeaderViewModel.IsFavorite = false;
        }

        public void Mute(int id)
        {
            PlurkHeaderViewModel.IsUnreadInt = (int)UnreadStatus.Muted;
        }

        public void Unmute(int id)
        {
            // Read and Unmute
            PlurkHeaderViewModel.IsUnreadInt = (int)UnreadStatus.Read;
        }

        public void SetAsRead(int id)
        {
            PlurkHeaderViewModel.IsUnreadInt = (int)UnreadStatus.Read;
        }

        #endregion
    }
}
