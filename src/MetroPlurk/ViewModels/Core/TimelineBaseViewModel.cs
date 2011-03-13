using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Entities;

namespace MetroPlurk.ViewModels
{
    [NotifyForAll]
    public abstract class TimelineBaseViewModel<TSource> : Screen
        where TSource : ITimeline
    {
        #region Fields
        protected readonly INavigationService NavigationService;
        protected readonly IProgressService ProgressService;
        protected readonly IPlurkService PlurkService;
        private IDisposable _requestHandler;
        private DateTime _timeBase;
        private TSource _lastResult;
        private WeakReference _scrollCache;
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
                if (Items == null || Items.IsEmpty())
                {
                    return 0.0;
                }
                return IsHasMore ? 1.0 : 0.0;
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

        protected TimelineBaseViewModel
            (INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            string progressMessage = "Loading")
        {
            NavigationService = navigationService;
            ProgressService = progressService;
            PlurkService = plurkService;

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
            Request(observable, true);
        }

        public void RequestMore()
        {
            if (RequestMoreHandler == null)
            {
                IsHasMore = false;
                return;
            }
            Request(RequestMoreHandler(_lastResult), false);
        }

        private void Request(IObservable<TSource> observable, bool clear)
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

                var result = plurks.Zip();
                if (result == null || result.IsEmpty())
                {
                    if (clear)
                    {
                        Execute.OnUIThread(() => Message = "This timeline is empty.");
                    }
                    IsHasMore = false;
                }
                else
                {
                    Items.AddRange(result.Select(plurk => new PlurkItemViewModel()
                    {
                        Id = plurk.Plurk.Id,
                        UserId = plurk.Plurk.UserId,
                        Username = plurk.User.DisplayNameOrNickName,
                        Qualifier = plurk.Plurk.QualifierTextView(),
                        PostDate = plurk.Plurk.PostDate,
                        PostTimeFromNow = _timeBase - plurk.Plurk.PostDate,
                        Content = plurk.Plurk.Content,
                        ContentRaw = plurk.Plurk.ContentRaw,
                        AvatarView = plurk.User.AvatarBig,
                        IsFavorite = plurk.Plurk.Favorite,
                        QualifierEnum = plurk.Plurk.Qualifier,
                        ResponseCount = plurk.Plurk.ResponseCount,
                        IsUnread = plurk.Plurk.IsUnread,
                        NoComments = plurk.Plurk.NoComments,
                        ContextMenuEnabled = PlurkService.IsLoaded,
                    }));
                    
                    if (IsHasMoreHandler != null)
                    {
                        IsHasMore = IsHasMoreHandler(plurks);
                    }
                }
            }, () => Execute.OnUIThread(() => ProgressService.Hide()));
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