﻿using System;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels.Main;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
{
    // Content may vary depending on user.
    [NotifyForAll]
    public sealed class PlurkMainPageViewModel : PlurkAppBarPage
    {
        private readonly TimelineViewModel _timeline;
        private readonly MyPlurksViewModel _myPlurksViewModel;
        private readonly PrivatePlurksViewModel _privatePlurksViewModel;
        private readonly RespondedPlurksViewModel _respondedPlurksViewModel;
        private readonly LikedPlurksViewModel _likedPlurksViewModel;

        public string Username { get; set; }

        public string UserAvatar { get; set; }

        public bool NewPost { get; set; }

        public PlurkMainPageViewModel(
            INavigationService navigationService,
            IPlurkService plurkService,
            LoginViewModel loginViewModel,
            TimelineViewModel timeline,
            MyPlurksViewModel myPlurksViewModel,
            PrivatePlurksViewModel privatePlurksViewModel,
            RespondedPlurksViewModel respondedPlurksViewModel,
            LikedPlurksViewModel likedPlurksViewModel)
            : base(navigationService, plurkService, loginViewModel)
        {
            _timeline = timeline;
            _myPlurksViewModel = myPlurksViewModel;
            _privatePlurksViewModel = privatePlurksViewModel;
            _respondedPlurksViewModel = respondedPlurksViewModel;
            _likedPlurksViewModel = likedPlurksViewModel;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.Add(_timeline);
            Items.Add(_myPlurksViewModel);
            Items.Add(_privatePlurksViewModel);
            Items.Add(_respondedPlurksViewModel);
            Items.Add(_likedPlurksViewModel);
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
                _timeline.ScrollToTop();
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

        public void AllPlurksAppBar()
        {
            ActivateItem(_timeline);
        }

        #endregion
    }
}
