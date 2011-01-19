using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using Plurto.Entities;

namespace MetroPlurk.ViewModels
{
    public abstract class TimelineBaseViewModel<TSource> : Screen
        where TSource : ITimeline
    {
        protected readonly INavigationService NavigationService;
        protected readonly IProgressService ProgressService;
        protected readonly IPlurkService PlurkService;
        private IDisposable _requestHandler;
        public string ProgressMessage { get; set; }

        protected TimelineBaseViewModel
            (INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            string progressMessage="Loading")
        {
            NavigationService = navigationService;
            ProgressService = progressService;
            PlurkService = plurkService;

            ProgressMessage = progressMessage;

            Items = new BindableCollection<PlurkItemViewModel>();
        }

        public IObservableCollection<PlurkItemViewModel> Items { get; set; }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message == value) return;
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        private bool _isHasMore;

        public bool IsHasMore
        {
            get { return _isHasMore; }
            set
            {
                if (_isHasMore == value) return;
                _isHasMore = value;
                NotifyOfPropertyChange(() => IsHasMore);
            }
        }

        private double _isHasMoreOpacity;

        public double IsHasMoreOpacity
        {
            get
            {
                if (Items == null || Items.IsEmpty())
                {
                    return 0.0;
                }
                return _isHasMoreOpacity;
            }
            set
            {
                if (_isHasMoreOpacity == value) return;
                _isHasMoreOpacity = value;
                NotifyOfPropertyChange(() => IsHasMoreOpacity);
            }
        }

        protected Func<TSource, bool> IsHasMoreHandler { get; set; }

        protected Func<TSource, IObservable<TSource>> RequestMoreHandler { get; set; }

        private DateTime _timeBase;
        private TSource _lastResult;

        private int _listSelectedIndex = -1;

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

        public void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (ListSelectedIndex == -1)
            {
                return;
            }
            var item = Items[ListSelectedIndex];
            var location = new PlurkLocation(item);
            NavigationService.Navigate(location);
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

        private void Request
            (IObservable<TSource> observable,
            bool clear)
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
                Items.Clear();
                IsHasMoreOpacity = 0.0;
            }

            _requestHandler = observable.
                Timeout(TimeSpan.FromSeconds(15)).PlurkException(error =>
            {
                IsHasMore = tempIsHasMore;
            }).Subscribe(plurks =>
            {
                _lastResult = plurks;
                if (IsHasMoreHandler != null)
                {
                    IsHasMore = IsHasMoreHandler(plurks);
                }

                var result = plurks.Zip();
                if (result.IsEmpty())
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
                        Username = plurk.User.DisplayNameOrNickName,
                        Qualifier = plurk.Plurk.QualifierTranslatedOrDefault,
                        PostDate = plurk.Plurk.PostDate,
                        PostTimeFromNow = _timeBase - plurk.Plurk.PostDate,
                        ContentRaw = plurk.Plurk.ContentRaw,
                        AvatarView = plurk.User.AvatarBig,
                        IsFavorite = plurk.Plurk.Favorite,
                        QualifierEnum = plurk.Plurk.Qualifier,
                        ResponseCount = plurk.Plurk.ResponseCount,
                        IsUnread = plurk.Plurk.IsUnread,
                        NoComments = plurk.Plurk.NoComments,
                        ContextMenuEnabled = PlurkService.IsLoaded,
                    }));
                }
                IsHasMoreOpacity = IsHasMore ? 1.0 : 0.0;
            }, () => Execute.OnUIThread(() => ProgressService.Hide()));
        }

        public void CancelRequest()
        {
            Execute.OnUIThread(() => ProgressService.Hide());
            _requestHandler.Dispose();
        }
    }
}