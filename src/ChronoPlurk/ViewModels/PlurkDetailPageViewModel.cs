﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels.Core;
using ChronoPlurk.Views;
using NotifyPropertyWeaver;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class PlurkDetailPageViewModel : LoginAvailablePage, INavigationInjectionRedirect, IPlurkHolder
    {
        private IPlurkService PlurkService { get; set; }

        private PlurkDetailHeaderViewModel PlurkHeaderViewModel { get { return PlurkDetailViewModel.DetailHeader; } }

        protected PlurkHolderService PlurkHolderService { get; set; }
        
        public PlurkDetailViewModel PlurkDetailViewModel { get; private set; }

        public PlurkDetailFooterViewModel ReplyViewModel { get; private set; }

        public Visibility ReplyVisibility { get; set; }
        
        public PlurkDetailPageViewModel
            (IPlurkService plurkService,
            PlurkHolderService plurkHolderService,
            PlurkDetailViewModel plurkDetailViewModel,
            PlurkDetailFooterViewModel replyViewModel,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            PlurkHolderService = plurkHolderService;
            PlurkService = plurkService;
            PlurkDetailViewModel = plurkDetailViewModel;
            ReplyViewModel = replyViewModel;
            ReplyVisibility = Visibility.Collapsed;
        }

        protected override void OnActivate()
        {
            if (!ReplyViewModel.OpeningPhotoChooser)
            {
                PlurkDetailViewModel.RefreshOnActivate = true;
                ActivateItem(PlurkHeaderViewModel);
                ActivateItem(PlurkDetailViewModel);
                // Read Plurk
                PlurkHolderService.SetAsRead(PlurkHeaderViewModel.PlurkId);
            }
            else
            {
                ReplyViewModel.OpeningPhotoChooser = false;
                ReplyViewModel.ResponseFocus = true;
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
            if (String.IsNullOrWhiteSpace(ReplyViewModel.PostContent))
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
            if (String.IsNullOrWhiteSpace(ReplyViewModel.PostContent))
            {
                ReplyVisibility = Visibility.Visible;
                ReplyViewModel.ResponseFocus = true;
            }
            else
            {
                ReplyViewModel.Reply();
            }
        }

        public void PhotosAppBar()
        {
            ReplyViewModel.InsertPhoto();
        }

        public void LikeAppBar()
        {
            var isLike = PlurkHeaderViewModel.IsFavorite;
            if (isLike)
            {
                PlurkService.Unfavorite(PlurkHeaderViewModel.PlurkId);
            }
            else
            {
                PlurkService.Favorite(PlurkHeaderViewModel.PlurkId);
            }

            PlurkDetailViewModel.ScrollToTop();
            ReloadAppBar();
        }

        public void MuteAppBar()
        {
            var isMute = PlurkHeaderViewModel.IsUnread == UnreadStatus.Muted;
            if (isMute)
            {
                PlurkService.Unmute(PlurkHeaderViewModel.PlurkId);
            }
            else
            {
                PlurkService.Mute(PlurkHeaderViewModel.PlurkId);
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

        public IEnumerable<long> PlurkIds
        {
            get { return new[] { PlurkHeaderViewModel.PlurkId }; }
        }

        public void Favorite(long plurkId)
        {
            PlurkHeaderViewModel.IsFavorite = true;
            UpdateLikeButton(PlurkHeaderViewModel.LikeText);
        }

        public void Unfavorite(long plurkId)
        {
            PlurkHeaderViewModel.IsFavorite = false;
            UpdateLikeButton(PlurkHeaderViewModel.LikeText);
        }

        public void Mute(long plurkId)
        {
            PlurkHeaderViewModel.IsUnreadInt = (int)UnreadStatus.Muted;
            UpdateMuteButton(PlurkHeaderViewModel.MuteText);
        }

        public void Unmute(long plurkId)
        {
            // Read and Unmute
            PlurkHeaderViewModel.IsUnreadInt = (int)UnreadStatus.Read;
            UpdateMuteButton(PlurkHeaderViewModel.MuteText);
        }

        public void SetAsRead(long plurkId)
        {
            PlurkHeaderViewModel.IsUnreadInt = (int)UnreadStatus.Read;
            UpdateMuteButton(PlurkHeaderViewModel.MuteText);
        }

        #endregion

    }
}
