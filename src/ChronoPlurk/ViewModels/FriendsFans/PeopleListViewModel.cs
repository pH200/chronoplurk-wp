using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Helpers;
using ChronoPlurk.Services;
using PropertyChanged;
using Plurto.Commands;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.FriendsFans
{
    [ImplementPropertyChanged]
    public class PeopleListViewModel : Screen, IRefreshSync
    {
        private IDisposable _requestHandler;

        public enum RequestForType
        {
            Friends, Fans
        }
        protected IPlurkService PlurkService { get; set; }

        protected IProgressService ProgressService { get; set; }

        protected INavigationService NavigationService { get; set; }

        protected Func<IList<User>, bool> IsHasMoreHandler { get; set; }

        protected Func<IObservable<IList<User>>> RequestMoreHandler { get; set; }

        public IObservableCollection<PeopleItemViewModel> Items { get; set; }

        public bool RefreshOnActivate { get; set; }

        public RequestForType RequestFor { get; set; }

        public bool IsHasMore { get; set; }

        [DependsOn("IsHasMore")]
        public double IsHasMoreOpacity
        {
            get
            {
                if (Items.IsNullOrEmpty())
                {
                    return 0.0;
                }
                else
                {
                    return IsHasMore ? 1.0 : 0.0;
                }
            }
        }

        public int Limit { get { return 10; } }

        public string Message { get; set; }

        public string ProgressMessage { get; set; }

        public PeopleListViewModel(
            IPlurkService plurkService,
            IProgressService progressService,
            INavigationService navigationService)
        {
            PlurkService = plurkService;
            ProgressService = progressService;
            NavigationService = navigationService;

            IsHasMoreHandler = list => !(list.Count < Limit);

            Items = new AdditiveBindableCollection<PeopleItemViewModel>();
        }

        protected override void OnActivate()
        {
            if (RefreshOnActivate)
            {
                RefreshOnActivate = false;
                RefreshSync();
            }
            base.OnActivate();
        }

        public void RefreshSync()
        {
            if (RequestFor == RequestForType.Friends)
            {
                RefreshFriends();
            }
            else
            {
                RefreshFans();
            }
        }

        public void RequestMore()
        {
            if (IsHasMore)
            {
                if (RequestMoreHandler == null)
                {
                    IsHasMore = false;
                    return;
                }
                InternalRequest(RequestMoreHandler(), false);
            }
        }

        public void OnUserTapped(object dataContext)
        {
            var user = dataContext as PeopleItemViewModel;
            if (user != null)
            {
                NavigationService.GotoProfilePage(user.Id, user.NickName, user.AvatarView.ToString());
            }
        }

        private void RefreshFriends()
        {
            var getObservable = FriendsFansCommand.GetFriendsByOffset(PlurkService.UserId).Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = () =>
                                 FriendsFansCommand.GetFriendsByOffset(PlurkService.UserId, Items.Count, Limit)
                                     .Client(PlurkService.Client).ToObservable();
            Request(getObservable);
        }

        private void RefreshFans()
        {
            var getObservable = FriendsFansCommand.GetFansByOffset(PlurkService.UserId).Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = () =>
                                 FriendsFansCommand.GetFansByOffset(PlurkService.UserId, Items.Count, Limit)
                                     .Client(PlurkService.Client).ToObservable();
            Request(getObservable);
        }

        private void Request(IObservable<IList<User>> observable)
        {
            InternalRequest(observable, true);
        }

        private void InternalRequest(IObservable<IList<User>> observable, bool clear)
        {
            if (_requestHandler != null)
            {
                _requestHandler.Dispose();
            }

            Message = string.Empty;
            var tempIsHasMore = IsHasMore;
            IsHasMore = false;
            if (clear)
            {
                Clear();
            }

            _requestHandler = observable
                .DoProgress(ProgressService, ProgressMessage)
                .PlurkException(error =>
                {
                    IsHasMore = tempIsHasMore;
                }, expectedTimeout: DefaultConfiguration.TimeoutTimeline)
                .Subscribe(users =>
                {
                    var result = users;
                    if (result.IsNullOrEmpty())
                    {
                        if (clear)
                        {
                            //Execute.OnUIThread(() => Message = AppResources.emptyTimeline);
                        }
                        IsHasMore = false;
                    }
                    else
                    {
                        Items.AddRange(Map(result));

                        if (IsHasMoreHandler != null)
                        {
                            IsHasMore = IsHasMoreHandler(users);
                        }
                    }
                });
        }

        private static IEnumerable<PeopleItemViewModel> Map(IEnumerable<User> result)
        {
            return result.Select(user => new PeopleItemViewModel()
            {
                Id = user.Id,
                AvatarView = MapAvatarToUri(user.AvatarBig),
                BirthdayPrivacy = user.BirthdayPrivacy,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Location = user.Location,
                Karma = user.Karma,
                Username = user.DisplayNameOrNickName,
                NickName = user.NickName,
            });
        }

        // TODO: Create helper method.
        private static Uri MapAvatarToUri(string avatar)
        {
            if (avatar.Contains("www.plurk.com/static/default_"))
            {
                return new Uri("Resources/Avatar/default_big.jpg", UriKind.Relative);
            }
            else
            {
                return new Uri(avatar, UriKind.Absolute);
            }
        }

        private void Clear()
        {
            Items.Clear();
        }
    }
}
