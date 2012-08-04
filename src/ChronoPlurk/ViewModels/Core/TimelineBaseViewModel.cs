using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using ChronoPlurk.ViewModels.Core;
using ChronoPlurk.Views.PlurkControls;
using NotifyPropertyWeaver;
using Plurto.Core;
using Plurto.Entities;
using WP7Contrib.View.Controls.BindingListener;
using WP7Contrib.View.Controls.Extensions;

namespace ChronoPlurk.ViewModels
{
    public interface ITimelineViewModel
    {
        IEnumerable<long> GetUnreadPlurkIds();
    }

    [NotifyForAll]
    public abstract class TimelineBaseViewModel<TSource> : Screen, IPlurkHolder, ITimelineViewModel
        where TSource : class, ITimeline
    {
        #region Fields
        private IDisposable _requestHandler;
        private DateTime _timeBase = DateTime.UtcNow;
        private TSource _lastResult;
        private WeakReference _scrollCache;
        private bool _isCachedItemsLoaded;
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

        protected bool IsCompareIdInsteadOfPlurkId { get; set; }

        protected bool EnableHyperlink { get; set; }

        public bool IgnoreSelection { get; set; }

        public string CachingId { get; set; }

        public IEnumerable<PlurkItemViewModel> PrecachedItems { get; set; }

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
            LoadCachedItems();
        }

        protected override void OnActivate()
        {
            LoadPrecachedItems();

            base.OnActivate();
        }

        protected void LoadCachedItems()
        {
            if (Items.Count == 0)
            {
                if (PrecachedItems != null)
                {
                    LoadPrecachedItems();
                }
                else
                {
                    var filename = GetSerializationFilename();
                    if (filename != null)
                    {
                        var list = IsoSettings.DeserializeLoad(filename) as List<PlurkItemViewModel>;
                        if (list != null)
                        {
                            Items.AddRange(list);
                            _isCachedItemsLoaded = true;
                        }
                    }
                }
            }
        }

        private void LoadPrecachedItems()
        {
            if (PrecachedItems != null && Items.Count == 0)
            {
                Items.AddRange(PrecachedItems);
                PrecachedItems = null; // Release
                _isCachedItemsLoaded = true;
            }
            else
            {
                PrecachedItems = null;
            }
        }

        protected override void OnViewLoaded(object view)
        {
            if (!DisableTimelinePlurkHolder)
            {
                var service = IoC.Get<PlurkHolderService>();
                service.Add(this);
            }
            var uiView = view as UIElement;
            if (uiView != null)
            {
                SubscribeTimelineScroll(uiView);
            }
            base.OnViewLoaded(view);
        }

        private void SubscribeTimelineScroll(DependencyObject uiView)
        {
            var timeline = uiView.FindVisualChildByName<TimelineControl>("Timeline");
            var sv = timeline.GetVisualDescendants()
                .OfType<ScrollViewer>()
                .First();
            var listener = new DependencyPropertyListener();
            listener.ValueChanged += (sender, args) =>
            {
                if (!IsHasMore)
                {
                    return;
                }
                var isBottom = (sv.VerticalOffset + 7) >= sv.ScrollableHeight;
                if (isBottom)
                {
                    var settings = IoC.Get<SettingsService>();
                    if (settings.GetIsInfiniteScroll())
                    {
                        RequestMore();
                    }
                }
            };
            listener.Attach(sv, new Binding("VerticalOffset") {Source = sv});
        }

        /// <summary>
        /// Get filename for serialized binary's filename. Null return is possible.
        /// </summary>
        /// <returns>Filename or null</returns>
        public string GetSerializationFilename()
        {
            var userId = PlurkService.UserId;
            if (CachingId != null && userId > 0)
            {
                return string.Format("timeline_cache_{0}_{1}.bin", userId, CachingId);
            }
            else
            {
                return null;
            }
        }

        public virtual void OnItemTap(object dataContext)
        {
            if (!IgnoreSelection)
            {
                var item = dataContext as PlurkItemViewModel;
                if (item != null)
                {
                    var location = new PlurkLocation(item);
                    NavigationService.Navigate(location);
                    PlurkContentStorageService.AddOrReplace(item.PlurkId, item.ContentHtml);
                }
            }
        }

        public void Clear()
        {
            Items.Clear();
            _lastResult = null;
            IsHasMore = false;
        }

        public void Request(IObservable<TSource> observable)
        {
            _timeBase = DateTime.UtcNow;
            InternalRequest(observable, true);
        }

        public void AppendRequest(IObservable<TSource> observable, SpecialFallback<TSource> specialFallback = null)
        {
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
            if (clear && !_isCachedItemsLoaded)
            {
                Clear();
            }

            _requestHandler = observable
                .PlurkException(error =>
                {
                    IsHasMore = tempIsHasMore;
                }, expectedTimeout: DefaultConfiguration.TimeoutTimeline)
                .Subscribe(plurks =>
                {
                    if (specialFallback != null)
                    {
                        if (specialFallback.Predicate(plurks))
                        {
                            specialFallback.Fallback();
                            return;
                        }
                    }

                    _lastResult = plurks;

                    var result = plurks.ToUserPlurks();
                    if (!_isCachedItemsLoaded && result != null)
                    {
                        result = RemoveDuplicateResult(result);
                    }

                    if (result.IsNullOrEmpty())
                    {
                        if (clear)
                        {
                            if (_isCachedItemsLoaded)
                            {
                                Items.Clear();
                                _isCachedItemsLoaded = false;
                            }
                            Message = AppResources.emptyTimeline;
                        }
                        IsHasMore = false;
                    }
                    else
                    {
                        Execute.OnUIThread(() => ProgressService.Show(AppResources.msgUpdatingTimeline));
                        var items = MapUserPlurkToPlurkItemViewModel(result, plurks);
                        if (_isCachedItemsLoaded)
                        {
                            var collection = new AdditiveBindableCollection<PlurkItemViewModel>(items);
                            Execute.OnUIThread(() => Items = collection); // Fix LongList behavior
                            _isCachedItemsLoaded = false;
                            CacheItems(collection); // Cache
                        }
                        else
                        {
                            Items.AddRange(items);
                            CacheItems(Items); // Cache
                        }

                        if (IsHasMoreHandler != null)
                        {
                            IsHasMore = IsHasMoreHandler(plurks);
                        }
                    }
                }, () =>
                {
                    Execute.OnUIThread(() => ProgressService.Hide());
                    OnRequestCompleted(_lastResult);
                });
        }

        private void CacheItems(IObservableCollection<PlurkItemViewModel> items)
        {
            if (items.Count > 0)
            {
                var filename = GetSerializationFilename();
                if (filename != null)
                {
                    ThreadEx.OnThreadPool(() =>
                    {
                        var list = items.Take(DefaultConfiguration.CachedItemsCount).ToList();
                        IsoSettings.SerializeStore(list, filename);
                    });
                }
            }
        }

        protected virtual void OnRequestCompleted(TSource lastResult)
        {
        }

        private IEnumerable<UserPlurk> RemoveDuplicateResult(IEnumerable<UserPlurk> result)
        {
            return result.Where(p => Items.All(lastPlurk =>
            {
                if (IsCompareIdInsteadOfPlurkId && p.Plurk.Id == 0)
                {
                    return true;
                }
                var compareId = IsCompareIdInsteadOfPlurkId
                                    ? p.Plurk.Id
                                    : p.Plurk.PlurkId;
                return lastPlurk.PlurkId != compareId;
            }));
        }

        private IEnumerable<PlurkItemViewModel> MapUserPlurkToPlurkItemViewModel(IEnumerable<UserPlurk> result, TSource plurks)
        {
            Func<UserPlurk, string> getReplurkerName = plurk =>
            {
                if (plurk.Plurk.ReplurkerId.HasValue)
                {
                    User user;
                    if (plurks.Users.TryGetValue(plurk.Plurk.ReplurkerId.Value, out user))
                    {
                        return user.DisplayNameOrNickName;
                    }
                }
                return null;
            };
            return result.Select(plurk => new PlurkItemViewModel()
            {
                PlurkId = plurk.Plurk.PlurkId,
                Id = plurk.Plurk.Id,
                UserId = plurk.User.Id, // Plurk.UserId may return client's id if logged in.
                ClientUserId = PlurkService.UserId,
                Username = plurk.User.DisplayNameOrNickName,
                NickName = plurk.User.NickName,
                Qualifier = plurk.Plurk.QualifierTextView(),
                PostDate = plurk.Plurk.PostDate,
                PostTimeFromNow = _timeBase - plurk.Plurk.PostDate,
                ContentHtml = plurk.Plurk.Content,
                ContentRaw = plurk.Plurk.ContentRaw,
                AvatarView = AvatarHelper.MapAvatar(plurk.User),
                IsFavorite = plurk.Plurk.Favorite,
                QualifierEnum = plurk.Plurk.Qualifier,
                ResponseCount = plurk.Plurk.ResponseCount,
                IsUnread = plurk.Plurk.IsUnread,
                NoComments = plurk.Plurk.NoComments,
                PlurkType = plurk.Plurk.PlurkType,
                ContextMenuEnabled = PlurkService.IsLoaded,
                EnableHyperlink = this.EnableHyperlink,
                IsReplurkable = plurk.Plurk.Replurkable,
                IsReplurked = (plurk.Plurk.Replurked == true),
                ReplurkerName = getReplurkerName(plurk),
            });
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
                scroll.ScrollToVerticalOffset(scroll.ScrollableHeight);
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
            
            var view = GetView() as UIElement;
            if (view == null)
            {
                return null;
            }
            var scroll = view.FindChildOfType<ScrollViewer>();
            
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

        public IEnumerable<long> GetUnreadPlurkIds()
        {
            if (Items != null)
            {
                return from plurk in Items
                       where plurk.IsUnread == UnreadStatus.Unread
                       select plurk.PlurkId;
            }
            return null;
        }

        #region IPlurkHolder

        public IEnumerable<long> PlurkIds
        {
            get { return Items.Select(item => item.PlurkId); }
        }

        private void SearchAndAction(long plurkId, Action<PlurkItemViewModel> action)
        {
            var item = Items.FirstOrDefault(p => plurkId == p.PlurkId);
            if (item != null)
            {
                action(item);
            }
        }

        public void Favorite(long plurkId)
        {
            SearchAndAction(plurkId, item => item.IsFavorite = true);
        }

        public void Unfavorite(long plurkId)
        {
            SearchAndAction(plurkId, item => item.IsFavorite = false);
        }

        public virtual void Mute(long plurkId)
        {
            SearchAndAction(plurkId, item => item.IsUnread = UnreadStatus.Muted);
        }

        public void Unmute(long plurkId)
        {
            SearchAndAction(plurkId, item => item.IsUnread = UnreadStatus.Read);
        }

        public virtual void SetAsRead(long plurkId)
        {
            SearchAndAction(plurkId, item => item.IsUnread = UnreadStatus.Read);
        }

        public void Replurk(long plurkId)
        {
            SearchAndAction(plurkId, item => item.IsReplurked = true);
        }

        public void Unreplurk(long plurkId)
        {
            SearchAndAction(plurkId, item => item.IsReplurked = false);
        }

        #endregion


        #region Context Menu

        public void MenuMute(object dataContext)
        {
            var item = dataContext as PlurkItemViewModel;
            if (item != null)
            {
                var service = IoC.Get<IPlurkService>();
                if (item.IsUnread == UnreadStatus.Muted)
                {
                    service.Unmute(item.PlurkId);
                }
                else
                {
                    service.Mute(item.PlurkId);
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
                    service.Unfavorite(item.PlurkId);
                }
                else
                {
                    service.Favorite(item.PlurkId);
                }
            }
        }

        public void MenuReplurk(object dataContext)
        {
            var item = dataContext as PlurkItemViewModel;
            if (item != null)
            {
                var service = IoC.Get<IPlurkService>();
                if (item.IsReplurked)
                {
                    service.Unreplurk(item.PlurkId);
                }
                else
                {
                    service.Replurk(item.PlurkId);
                }
            }
        }

        public void MenuDelete(object dataContext)
        {
            var item = dataContext as PlurkItemViewModel;
            if (item != null)
            {
                if (Items.Remove(item))
                {
                    var service = IoC.Get<IPlurkService>();
                    if (!IsCompareIdInsteadOfPlurkId)
                    {
                        service.Delete(item.PlurkId); // delete plurk
                    }
                    else
                    {
                        service.DeleteResponse(item.Id, item.PlurkId);
                    }
                }
            }
        }

        #endregion
    }
}
