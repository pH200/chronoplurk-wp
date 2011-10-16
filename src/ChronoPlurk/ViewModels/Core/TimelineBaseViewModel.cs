using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using NotifyPropertyWeaver;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public abstract class TimelineBaseViewModel<TSource> : Screen
        where TSource : ITimeline
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
            IPlurkContentStorageService plurkContentStorageService,
            string progressMessage = "Loading")
        {
            NavigationService = navigationService;
            ProgressService = progressService;
            PlurkService = plurkService;
            PlurkContentStorageService = plurkContentStorageService;

            ProgressMessage = progressMessage;

            Items = new AdditiveBindableCollection<PlurkItemViewModel>();
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

            _requestHandler = observable.
                Timeout(TimeSpan.FromSeconds(15)).PlurkException(error =>
                {
                    IsHasMore = tempIsHasMore;
                }).Subscribe(plurks =>
                {
                    _lastResult = plurks;

                    if (specialFallback != null)
                    {
                        if (specialFallback.Predicate(plurks))
                        {
                            specialFallback.Fallback();
                            return;
                        }
                    }

                    var result = plurks.ToUserPlurks();
                    if (result.IsNullOrEmpty())
                    {
                        if (clear)
                        {
                            Execute.OnUIThread(() => Message = "This timeline is empty.");
                        }
                        IsHasMore = false;
                    }
                    else
                    {
                        Execute.OnUIThread(() => ProgressService.Show("Updating Timeline"));
                        Items.AddRange(MapUserPlurkToPlurkItemViewModel(result));

                        if (IsHasMoreHandler != null)
                        {
                            IsHasMore = IsHasMoreHandler(plurks);
                        }
                    }
                }, () => Execute.OnUIThread(() => ProgressService.Hide()));
        }

        private IEnumerable<PlurkItemViewModel> MapUserPlurkToPlurkItemViewModel(IEnumerable<UserPlurk> result)
        {
            return result.Select(plurk => new PlurkItemViewModel()
            {
                Id = plurk.Plurk.Id,
                UserId = plurk.Plurk.UserId,
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
    }
}