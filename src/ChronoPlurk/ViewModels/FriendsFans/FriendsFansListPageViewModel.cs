using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels.FriendsFans
{
    [ImplementPropertyChanged]
    public class FriendsFansListPageViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public PeopleListViewModel FriendsListViewModel { get; set; }

        public PeopleListViewModel FansListViewModel { get; set; }

        public FriendsFansListPageViewModel()
        {
            var friends = IoC.Get<PeopleListViewModel>();
            friends.DisplayName = AppResources.friends;
            friends.ProgressMessage = AppResources.msgLoadingFriends;
            friends.RequestFor = PeopleListViewModel.RequestForType.Friends;
            friends.RefreshOnActivate = true;
            FriendsListViewModel = friends;
            Items.Add(FriendsListViewModel);
            
            var fans = IoC.Get<PeopleListViewModel>();
            fans.DisplayName = AppResources.fans;
            fans.ProgressMessage = AppResources.msgLoadingFans;
            fans.RequestFor = PeopleListViewModel.RequestForType.Fans;
            fans.RefreshOnActivate = true;
            FansListViewModel = fans;
            Items.Add(FansListViewModel);
        }

        protected override void OnInitialize()
        {
            ActivateItem(FriendsListViewModel);
            base.OnInitialize();
        }
    }
}
