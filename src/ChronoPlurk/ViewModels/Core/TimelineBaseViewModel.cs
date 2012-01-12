using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using ChronoPlurk.ViewModels.Core;
using NotifyPropertyWeaver;
using Plurto.Core;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public abstract class TimelineBaseViewModel<TSource> : Screen, IPlurkHolder
        where TSource : class, ITimeline
    {
        #region Fields
        private IDisposable _requestHandler;
        private DateTime _timeBase;
        private TSource _lastResult;
        private WeakReference _scrollCache;
        #endregion

        #region Services
        protected INavigationService NavigationService { get; set; }
        protected IProgressService ProgressService { get; set; }
        protected IPlurkService PlurkService { get; set; }
        protected IPlurkContentStorageService PlurkContentStorageService { get; set; }
        #endregion

        #region Empty Header and Footer

        private readonly EmptyViewModel _listHeader = new EmptyViewModel();
        public virtual object ListHeader { get { return _listHeader; } }

        private readonly EmptyViewModel _listFooter = new EmptyViewModel();
        public virtual object ListFooter { get { return _listFooter; } }

        #endregion

        /// <summary>
        /// Message in progess service.
        /// </summary>
        public string ProgressMessage { get; set; }

        public IObservableCollection<PlurkItemViewModel> Items { get; set; }

        public string Message { get; set; }

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

        protected Func<TSource, bool> IsHasMoreHandler { get; set; }

        protected Func<TSource, IObservable<TSource>> RequestMoreHandler { get; set; }

        protected bool DisableTimelinePlurkHolder { get; set; }

        #region ListSelectedIndex
        
        private int _listSelectedIndex = -1; // Must defualt as -1

        public int ListSelectedIndex
        {
            get { return _listSelectedIndex; }
            set
            {
                if (_listSelectedIndex == value) return;
                _listSelectedIndex = value;
                NotifyOfPropertyChange(() => ListSelectedIndex);
            }
        }

        public bool IgnoreSelection { get; set; }

        #endregion

        protected TimelineBaseViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService)
        {
            NavigationService = navigationService;
            ProgressService = progressService;
            PlurkService = plurkService;
            PlurkContentStorageService = plurkContentStorageService;

            ProgressMessage = AppResources.msgLoading;

            Items = new AdditiveBindableCollection<PlurkItemViewModel>();
        }

        protected override void OnViewLoaded(object view)
        {
            if (!DisableTimelinePlurkHolder)
            {
                var service = IoC.Get<PlurkHolderService>();
                service.Add(this);
            }
            base.OnViewLoaded(view);
        }

        public void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (IgnoreSelection)
            {
                return;
            }
            if (ListSelectedIndex == -1)
            {
                return;
            }
            var item = Items[ListSelectedIndex];
            var location = new PlurkLocation(item);
            NavigationService.Navigate(location);
            PlurkContentStorageService.AddOrReplace(item.Id, item.ContentHtml);

            ListSelectedIndex = -1;
        }

        public void Clear()
        {
            Items.Clear();
            _lastResult = null;
            IsHasMore = false;
        }

        public void Request(IObservable<TSource> observable)
        {
            _timeBase = DateTime.Now;
            InternalRequest(observable, true);
        }

        public void AppendRequest(IObservable<TSource> observable, SpecialFallback<TSource> specialFallback = null)
        {
            _timeBase = DateTime.Now;
            InternalRequest(observable, false, specialFallback);
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
                InternalRequest(RequestMoreHandler(_lastResult), false);
            }
        }

        private void InternalRequest(IObservable<TSource> observable, bool clear, SpecialFallback<TSource> specialFallback = null)
        {
            if (_requestHandler != null)
            {
                _requestHandler.Dispose();
            }

            ProgressService.Show(ProgressMessage);
            Message = string.Empty;
            var tempIsHasMore = IsHasMore;
            IsHasMore = false;
            if (clear)
            {
                Clear();
            }

            _requestHandler = observable
                .Timeout(DefaultConfiguration.TimeoutTimeline)
                .PlurkException(error =>
                {
                    IsHasMore = tempIsHasMore;
                }).Subscribe(plurks =>
                {
                    if (specialFallback != null)
                    {
                        if (specialFallback.Predicate(plurks))
                        {
                            specialFallback.Fallback();
                            return;
                        }
                    }

                    // Fix duplicate issue.
                    var skipFirst = false;
                    var duplicateState = CheckSingleDuplicatePlurk(plurks);
                    switch (duplicateState)
                    {
                        case DuplicateState.FirstAndMore:
                            skipFirst = true;
                            break;
                        case DuplicateState.FirstSingle:
                            IsHasMore = false;
                            return;
                    }

                    _lastResult = plurks;

                    var result = plurks.ToUserPlurks();
                    if (skipFirst)
                    {
                        result = result.Skip(1);
                    }
                    if (result.IsNullOrEmpty())
                    {
                        if (clear)
                        {
                            Execute.OnUIThread(() => Message = AppResources.emptyTimeline);
                        }
                        IsHasMore = false;
                    }
                    else
                    {
                        Execute.OnUIThread(() => ProgressService.Show(AppResources.msgUpdatingTimeline));
                        Items.AddRange(MapUserPlurkToPlurkItemViewModel(result));

                        if (IsHasMoreHandler != null)
                        {
                            IsHasMore = IsHasMoreHandler(plurks);
                        }
                    }
                }, () => Execute.OnUIThread(() => ProgressService.Hide()));
        }

        enum DuplicateState
        {
            None, FirstAndMore, FirstSingle
        }

        /// <summary>
        /// Fix duplicate plurk issue.
        /// </summary>
        /// <param name="plurks">Plurks from request handler.</param>
        /// <returns>DuplicateState</returns>
        private DuplicateState CheckSingleDuplicatePlurk(TSource plurks)
        {
            if (_lastResult != null)
            {
                var lastBottom = _lastResult.Plurks.LastOrDefault();
                if (lastBottom != null)
                {
                    var currentTop = plurks.Plurks.FirstOrDefault();
                    if (currentTop != null)
                    {
                        if (lastBottom.Id == currentTop.Id)
                        {
                            if (plurks.Plurks.Count == 1)
                            {
                                return DuplicateState.FirstSingle;
                            }
                            else
                            {
                                return DuplicateState.FirstAndMore;
                            }
                        }
                    }
                }
            }
            return DuplicateState.None;
        }

        private IEnumerable<PlurkItemViewModel> MapUserPlurkToPlurkItemViewModel(IEnumerable<UserPlurk> result)
        {
            return result.Select(plurk => new PlurkItemViewModel()
            {
                Id = plurk.Plurk.Id,
                UserId = plurk.User.Id, // Plurk.UserId may return client's id if logged in.
                Username = plurk.User.DisplayNameOrNickName,
                Qualifier = plurk.Plurk.QualifierTextView(),
                PostDate = plurk.Plurk.PostDate,
                PostTimeFromNow = _timeBase - plurk.Plurk.PostDate,
                ContentHtml = plurk.Plurk.Content,
                ContentRaw = plurk.Plurk.ContentRaw,
                AvatarView = MapAvatarToUri(plurk.User.AvatarBig),
                IsFavorite = plurk.Plurk.Favorite,
                QualifierEnum = plurk.Plurk.Qualifier,
                ResponseCount = plurk.Plurk.ResponseCount,
                IsUnread = plurk.Plurk.IsUnread,
                NoComments = plurk.Plurk.NoComments,
                PlurkType = plurk.Plurk.PlurkType,
                ContextMenuEnabled = PlurkService.IsLoaded,
            });
        }

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

        public void CancelRequest()
        {
            Execute.OnUIThread(() => ProgressService.Hide());
            _requestHandler.Dispose();
        }

        public void ScrollToTop()
        {
            var scroll = FindScroll();
            if (scroll != null)
            {
                scroll.ScrollToVerticalOffset(0);
            }
        }

        public void ScrollToEnd()
        {
            var scroll = FindScroll();
            if (scroll != null)
            {
                scroll.ScrollToVerticalOffset(double.MaxValue);
            }
        }

        private ScrollViewer FindScroll()
        {
            if (_scrollCache != null)
            {
                var cachedScroll = _scrollCache.Target as ScrollViewer;
                if (cachedScroll != null)
                {
                    return cachedScroll;
                }
            }
            
            var view = GetView(null) as UIElement;
            if (view == null)
            {
                return null;
            }
            var scroll = view.FindVisualChildByName<ScrollViewer>("ListScroll");
            
            if (_scrollCache == null)
            {
                _scrollCache = new WeakReference(scroll, false);
            }
            else
            {
                _scrollCache.Target = scroll;
            }

            return scroll;
        }

        public IEnumerable<int> PlurkIds
        {
            get { return Items.Select(item => item.Id); }
        }

        private void SearchAndAction(int id, Action<PlurkItemViewModel> action)
        {
            var item = Items.FirstOrDefault(p => id == p.Id);
            if (item != null)
            {
                action(item);
            }
        }

        public void Favorite(int id)
        {
            SearchAndAction(id, item => item.IsFavorite = true);
        }

        public void Unfavorite(int id)
        {
            SearchAndAction(id, item => item.IsFavorite = false);
        }

        public void Mute(int id)
        {
            SearchAndAction(id, item => item.IsUnread = UnreadStatus.Muted);
        }

        public void Unmute(int id)
        {
            SearchAndAction(id, item => item.IsUnread = UnreadStatus.Read);
        }

        public void SetAsRead(int id)
        {
            SearchAndAction(id, item => item.IsUnread = UnreadStatus.Read);
        }

        #region Context Menu

        public void MenuMute(object dataContext)
        {
            var item = dataContext as PlurkItemViewModel;
            if (item != null)
            {
                var service = IoC.Get<IPlurkService>();
                if (item.IsUnread == UnreadStatus.Muted)
                {
                    service.Unmute(item.Id);
                }
                else
                {
                    service.Mute(item.Id);
                }
            }
        }

        public void MenuLike(object dataContext)
        {
            var item = dataContext as PlurkItemViewModel;
            if (item != null)
            {
                var service = IoC.Get<IPlurkService>();
                if (item.IsFavorite)
                {
                    service.Unfavorite(item.Id);
                }
                else
                {
                    service.Favorite(item.Id);
                }
            }
        }

        #endregion
    }
}
