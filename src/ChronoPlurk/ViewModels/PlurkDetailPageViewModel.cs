﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels.Core;
using ChronoPlurk.Views;
using PropertyChanged;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    [ImplementPropertyChanged]
    public class PlurkDetailPageViewModel : Conductor<IScreen>.Collection.OneActive, INavigationInjectionRedirect, IPlurkHolder
    {
        private IPlurkService PlurkService { get; set; }

        private PlurkDetailHeaderViewModel PlurkHeaderViewModel { get { return PlurkDetailViewModel.DetailHeader; } }

        protected PlurkHolderService PlurkHolderService { get; set; }
        
        public PlurkDetailViewModel PlurkDetailViewModel { get; private set; }

        public PlurkDetailReplyViewModel ReplyViewModel { get; private set; }

        public Visibility ReplyVisibility { get; set; }
        
        public PlurkDetailPageViewModel
            (IPlurkService plurkService,
            PlurkHolderService plurkHolderService,
            PlurkDetailViewModel plurkDetailViewModel,
            PlurkDetailReplyViewModel replyViewModel)
        {
            PlurkHolderService = plurkHolderService;
            PlurkService = plurkService;
            PlurkDetailViewModel = plurkDetailViewModel;
            replyViewModel.Parent = this;
            ReplyViewModel = replyViewModel;
            
            ReplyVisibility = Visibility.Collapsed;

            PlurkDetailViewModel.RefreshOnActivate = true;
        }

        #region Screen Events

        protected override void OnActivate()
        {
            if (ReplyVisibility != Visibility.Visible)
            {
                ActivateItem(PlurkHeaderViewModel);
                ActivateItem(PlurkDetailViewModel);
                // Read Plurk
                PlurkHolderService.SetAsRead(PlurkHeaderViewModel.PlurkId);
            }

            base.OnActivate();
        }

        protected override void OnViewLoaded(object view)
        {
            ReloadAppBar(view as PlurkDetailPage);
            PlurkHolderService.Add(this);
            SetBackKeyPress(view as PlurkDetailPage);

            base.OnViewLoaded(view);
        }

        #endregion

        #region Back Key Handling

        private void SetBackKeyPress(PlurkDetailPage view)
        {
            if (view != null)
            {
                view.OnViewBackKeyPress = () =>
                {
                    if (ReplyVisibility == Visibility.Visible)
                    {
                        if (ReplyViewModel.EmoticonVisibility == Visibility.Visible)
                        {
                            HideEmoticonVisibility();
                        }
                        else
                        {
                            HideReplyVisibility();
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
            }
        }

        public void OnBackKeyPress(CancelEventArgs e)
        {
            if (ReplyVisibility == Visibility.Visible)
            {
                HideReplyVisibility();
                e.Cancel = true;
            }
        }

        #endregion

        #region Reply View Methods

        public void ShowReplyVisibility()
        {
            ReplyVisibility = Visibility.Visible;
            ShowAppBarForReply();
            UpdateReplyButton();
        }

        public void HideReplyVisibility()
        {
            ReplyVisibility = Visibility.Collapsed;
            HideAppBarForReply();
            UpdateReplyButton();
        }

        public void ShowEmoticonVisibility()
        {
            var view = GetView() as PlurkDetailPage;
            if (view != null)
            {
                view.ReplyRowMax();
            }
            ReplyViewModel.EmoticonVisibility = Visibility.Visible;
            ReplyViewModel.LoadEmoticons();
        }

        public void HideEmoticonVisibility()
        {
            ReplyViewModel.EmoticonVisibility = Visibility.Collapsed;
            var view = GetView() as PlurkDetailPage;
            if (view != null)
            {
                view.ReplyRowAuto();
            }
        }

        #endregion

        #region Public Methods

        public long GetPlurkId()
        {
            return PlurkDetailViewModel.DetailHeader.PlurkId;
        }

        public void LoadNewComments()
        {
            PlurkDetailViewModel.LoadNewComments();
        }

        public void AddTextToReply(string value)
        {
            if (value == null)
            {
                return;
            }
            if (ReplyVisibility == Visibility.Visible)
            {
                if (ReplyViewModel.PostContent == null)
                {
                    ReplyViewModel.PostContent = value;
                }
                else
                {
                    ReplyViewModel.PostContent += value;
                }
            }
        }

        #endregion

        #region AppBar

        private void ReloadAppBar()
        {
            ReloadAppBar(GetView() as PlurkDetailPage);
        }

        private void ReloadAppBar(PlurkDetailPage view)
        {
            if (view == null)
            {
                return;
            }

            UpdateReplyButton(view);

            view.LikeButton.Text = PlurkHeaderViewModel.LikeText;
            view.MuteButton.Text = PlurkHeaderViewModel.MuteText;
            view.ReplurkButton.Text = PlurkHeaderViewModel.ReplurkText;
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
        
        public void UpdateReplurkButton(string text)
        {
            var view = GetView() as PlurkDetailPage;
            if (view != null)
            {
                view.ReplurkButton.Text = text;
            }
        }

        #region Reply Button

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
            if (ReplyVisibility == Visibility.Collapsed)
            {
                if (view.ReplyButton.IconUri != PlurkDetailPage.ReplyIconUri)
                {
                    view.ReplyButton.IconUri = PlurkDetailPage.ReplyIconUri;
                }
            }
            else
            {
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
        }

        public void ShowAppBarForReply()
        {
            var view = GetView() as PlurkDetailPage;
            if (view != null)
            {
                view.EmoticonButton.IsEnabled = true;
                view.PhotosButton.IsEnabled = true;
            }
        }

        public void HideAppBarForReply()
        {
            var view = GetView() as PlurkDetailPage;
            if (view != null)
            {
                view.EmoticonButton.IsEnabled = false;
                view.PhotosButton.IsEnabled = false;
                HideEmoticonVisibility();
            }
        }

        #endregion


        public void RefreshAppBar()
        {
            PlurkDetailViewModel.RefreshSync();
        }

        public void ReplyAppBar()
        {
            if (ReplyVisibility == Visibility.Collapsed)
            {
                ShowReplyVisibility();
                ReplyViewModel.ResponseFocus = true;
            }
            else
            {
                ReplyViewModel.Compose();
            }
        }

        public void EmoticonAppBar()
        {
            if (ReplyViewModel.EmoticonVisibility == Visibility.Visible)
            {
                HideEmoticonVisibility();
            }
            else
            {
                ShowEmoticonVisibility();
            }

            var view = GetView() as Control;
            if (view != null)
            {
                view.Focus();
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

        public void ReplurkAppBar()
        {
            var isReplurked = PlurkHeaderViewModel.IsReplurked;
            if (isReplurked)
            {
                PlurkService.Unreplurk(PlurkHeaderViewModel.PlurkId);
            }
            else
            {
                PlurkService.Replurk(PlurkHeaderViewModel.PlurkId);
            }
            PlurkDetailViewModel.ScrollToTop();
            ReloadAppBar();
        }

        public void ScrollToLatestAppBar()
        {
            this.PlurkDetailViewModel.ScrollToEnd();
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

        public bool CanReplurkAppBar()
        {
            return IsLoggedIn() &&
                   PlurkHeaderViewModel.IsReplurkable &&
                   PlurkService.UserId != PlurkHeaderViewModel.UserId;
        }

        private bool IsLoggedIn()
        {
            return PlurkService.IsLoaded;
        }
        #endregion

        #region INavigationInjectionRedirect

        public object GetRedirectedViewModel()
        {
            return PlurkDetailViewModel.ListHeader;
        }

        #endregion

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

        public void Replurk(long plurkId)
        {
            PlurkHeaderViewModel.IsReplurked = true;
            UpdateReplurkButton(PlurkHeaderViewModel.ReplurkText);
        }

        public void Unreplurk(long plurkId)
        {
            PlurkHeaderViewModel.IsReplurked = false;
            UpdateReplurkButton(PlurkHeaderViewModel.ReplurkText);
        }

        #endregion
    }
}
