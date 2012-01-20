using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels.Core;
using ChronoPlurk.Views;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    public class PlurkDetailPageViewModel : LoginAvailablePage, INavigationInjectionRedirect, IPlurkHolder
    {
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
            if (!PlurkDetailViewModel.DetailFooter.OpeningPhotoChooser)
            {
                PlurkDetailViewModel.RefreshOnActivate = true;
                ActivateItem(PlurkHeaderViewModel);
                ActivateItem(PlurkDetailViewModel);
                // Read Plurk
                PlurkHolderService.SetAsRead(PlurkHeaderViewModel.Id);
            }
            else
            {
                PlurkDetailViewModel.DetailFooter.OpeningPhotoChooser = false;
                PlurkDetailViewModel.ScrollToEnd();
                PlurkDetailViewModel.DetailFooter.ResponseFocus = true;
            }

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

            UpdateReplyButton(view);

            view.LikeButton.Text = PlurkHeaderViewModel.LikeText;
            view.MuteButton.Text = PlurkHeaderViewModel.MuteText;
        }

        public void UpdateLikeButton(string text)
        {
            var view = GetView() as PlurkDetailPage;
            if (view != null)
            {
                view.LikeButton.Text = text;
            }
        }

        public void UpdateMuteButton(string text)
        {
            var view = GetView() as PlurkDetailPage;
            if (view != null)
            {
                view.MuteButton.Text = text;
            }
        }

        public void UpdateReplyButton()
        {
            UpdateReplyButton(GetView() as PlurkDetailPage);
        }

        public void UpdateReplyButton(PlurkDetailPage view)
        {
            if (view == null)
            {
                return;
            }
            if (String.IsNullOrWhiteSpace(PlurkDetailViewModel.DetailFooter.PostContent))
            {
                if (view.ReplyButton.IconUri != PlurkDetailPage.ReplyIconUri)
                {
                    view.ReplyButton.IconUri = PlurkDetailPage.ReplyIconUri;
                }
            }
            else
            {
                if (view.ReplyButton.IconUri != PlurkDetailPage.CheckIconUri)
                {
                    view.ReplyButton.IconUri = PlurkDetailPage.CheckIconUri;
                }
            }
        }

        public void ShowPhotosAppBar()
        {
            var view = GetView() as PlurkDetailPage;
            if (view != null)
            {
                view.PhotosButton.IsEnabled = true;
            }
        }

        public void HidePhotosAppBar()
        {
            var view = GetView() as PlurkDetailPage;
            if (view != null)
            {
                view.PhotosButton.IsEnabled = false;
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
                PlurkDetailViewModel.DetailFooter.ResponseFocus = true;
            }
            else
            {
                PlurkDetailViewModel.DetailFooter.Reply();
            }
        }

        public void PhotosAppBar()
        {
            PlurkDetailViewModel.DetailFooter.InsertPhoto();
        }

        public void LikeAppBar()
        {
            var isLike = PlurkHeaderViewModel.IsFavorite;
            if (isLike)
            {
                PlurkService.Unfavorite(PlurkHeaderViewModel.Id);
            }
            else
            {
                PlurkService.Favorite(PlurkHeaderViewModel.Id);
            }

            PlurkDetailViewModel.ScrollToTop();
            ReloadAppBar();
        }

        public void MuteAppBar()
        {
            var isMute = PlurkHeaderViewModel.IsUnread == UnreadStatus.Muted;
            if (isMute)
            {
                PlurkService.Unmute(PlurkHeaderViewModel.Id);
            }
            else
            {
                PlurkService.Mute(PlurkHeaderViewModel.Id);
            }

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
            UpdateLikeButton(PlurkHeaderViewModel.LikeText);
        }

        public void Unfavorite(int id)
        {
            PlurkHeaderViewModel.IsFavorite = false;
            UpdateLikeButton(PlurkHeaderViewModel.LikeText);
        }

        public void Mute(int id)
        {
            PlurkHeaderViewModel.IsUnreadInt = (int)UnreadStatus.Muted;
            UpdateMuteButton(PlurkHeaderViewModel.MuteText);
        }

        public void Unmute(int id)
        {
            // Read and Unmute
            PlurkHeaderViewModel.IsUnreadInt = (int)UnreadStatus.Read;
            UpdateMuteButton(PlurkHeaderViewModel.MuteText);
        }

        public void SetAsRead(int id)
        {
            PlurkHeaderViewModel.IsUnreadInt = (int)UnreadStatus.Read;
            UpdateMuteButton(PlurkHeaderViewModel.MuteText);
        }

        #endregion

    }
}
