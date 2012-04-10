using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels.Core;
using ChronoPlurk.ViewModels.Main;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
{
    // Content may vary depending on user.
    [NotifyForAll]
    public sealed class PlurkMainPageViewModel : PlurkAppBarPage
    {
        private readonly TimelineViewModel _timeline;
        private readonly UnreadPlurksViewModel _unreadPlurksViewModel;
        private readonly MyPlurksViewModel _myPlurksViewModel;
        private readonly PrivatePlurksViewModel _privatePlurksViewModel;
        private readonly RespondedPlurksViewModel _respondedPlurksViewModel;
        private readonly LikedPlurksViewModel _likedPlurksViewModel;

        private SettingsService SettingsService { get; set; }
        private SharePickerService SharePickerService { get; set; }

        private FiltersOnOffPack _filters;

        public string Username { get; set; }

        public string UserAvatar { get; set; }

        public bool NewPost { get; set; }

        public PlurkMainPageViewModel(
            INavigationService navigationService,
            IPlurkService plurkService,
            SettingsService settingsService,
            SharePickerService sharePickerService,
            TimelineViewModel timeline,
            UnreadPlurksViewModel unreadPlurksViewModel,
            MyPlurksViewModel myPlurksViewModel,
            PrivatePlurksViewModel privatePlurksViewModel,
            RespondedPlurksViewModel respondedPlurksViewModel,
            LikedPlurksViewModel likedPlurksViewModel)
            : base(navigationService, plurkService)
        {
            SettingsService = settingsService;
            SharePickerService = sharePickerService;

            _timeline = timeline;
            _unreadPlurksViewModel = unreadPlurksViewModel;
            _myPlurksViewModel = myPlurksViewModel;
            _privatePlurksViewModel = privatePlurksViewModel;
            _respondedPlurksViewModel = respondedPlurksViewModel;
            _likedPlurksViewModel = likedPlurksViewModel;

            _filters = SettingsService.GetFiltersPack();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.Add(_timeline);
            ResetFilters();
            ActivateItem(_timeline);
        }

        private void ResetFilters()
        {
            while (Items.Count > 1)
            {
                Items.RemoveAt(1);
            }
            if (_filters.Unread)
            {
                Items.Add(_unreadPlurksViewModel);
            }
            if (_filters.My)
            {
                Items.Add(_myPlurksViewModel);
            }
            if (_filters.Private)
            {
                Items.Add(_privatePlurksViewModel);
            }
            if (_filters.Responded)
            {
                Items.Add(_respondedPlurksViewModel);
            }
            if (_filters.Liked)
            {
                Items.Add(_likedPlurksViewModel);
            }
        }

        protected override void OnActivate()
        {
            var newFilters = SettingsService.GetFiltersPack();
            if (newFilters != _filters)
            {
                _filters = newFilters;
                ResetFilters();
                RefreshAll(Items.Where(item => item != _timeline));
            }
            if (PlurkService.IsUserChanged)
            {
                PlurkService.IsUserChanged = false;
                _unreadPlurksViewModel.UnreadCount = 0;
                _unreadPlurksViewModel.RefreshUnreadCount();
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

            if (SharePickerService.ProcessAction)
            {
                NavigationService.GotoComposePage();
            }
        }

        private void RefreshAll<T>(IEnumerable<T> collection)
        {
            foreach (var screen in collection.OfType<IRefreshSync>())
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

        private void RefreshAll()
        {
            RefreshAll(Items);
            RefreshUnreadCount();
        }

        private void RefreshUnreadCount()
        {
            var unreadViewModel = Items.OfType<UnreadPlurksViewModel>().FirstOrDefault();
            if (unreadViewModel != null)
            {
                unreadViewModel.RequestUnreadCount();
            }
        }

        private void MarkAllAsRead(ITimelineViewModel item)
        {
            if (item != null)
            {
                var unreadIds = item.GetUnreadPlurkIds();
                if (unreadIds != null)
                {
                    var list = unreadIds.Distinct().ToList();
                    if (list.Count > 0)
                    {
                        PlurkService.MarkAsRead(list);

                        if (Items.Contains(_unreadPlurksViewModel))
                        {
                            _unreadPlurksViewModel.RemoveReadItems();
                        }
                    }
                }
            }
        }

        #region AppBar
        
        public void RefreshAppBar()
        {
            RefreshAll();
        }

        public void MarkAllAsReadAppBar()
        {
            MarkAllAsRead(ActiveItem as ITimelineViewModel);
        }

        public void FriendsAndFansAppBar()
        {
            NavigationService.Navigate(new Uri("/Views/FriendsFans/FriendsFansListPage.xaml", UriKind.Relative));
        }

        public void AllPlurksAppBar()
        {
            ActivateItem(_timeline);
        }

        #endregion
    }
}
