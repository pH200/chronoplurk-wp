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
using PropertyChanged;
using Plurto.Core;
using Plurto.Entities;
using WP7Contrib.View.Controls.BindingListener;
using WP7Contrib.View.Controls.Extensions;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.Windows.Controls.Primitives;

namespace ChronoPlurk.ViewModels
{
    public interface ITimelineViewModel
    {
        IEnumerable<long> GetUnreadPlurkIds();
    }

    [ImplementPropertyChanged]
    public abstract class TimelineBaseViewModel<TSource> : Screen, IPlurkHolder, ITimelineViewModel
        where TSource : class, ITimeline
    {
        #region Fields
        private IDisposable _requestHandler;
        private DateTime _timeBase = DateTime.UtcNow;
        private TSource _lastResult;
        private LongListSelector _longListSelector;
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

        public Brush ItemColor { get; set; } = PlurkResources.PhoneForegroundBrush;

        protected Func<TSource, bool> IsHasMoreHandler { get; set; }

        protected Func<TSource, IObservable<TSource>> RequestMoreHandler { get; set; }

        protected bool DisableTimelinePlurkHolder { get; set; }

        protected bool IsCompareIdInsteadOfPlurkId { get; set; }

        protected bool EnableHyperlink { get; set; }

        public bool IgnoreSelection { get; set; }

        public bool EnableCaching { get; set; }

        public string CachingId { get; set; }

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

        #region Screen Events

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

        #endregion

        #region Caching

        protected void LoadCachedItems()
        {
            if (Items.Count == 0)
            {
                var filename = GetSerializationFilename();
                if (filename != null)
                {
                    var list = IsoSettings.DeserializeLoad(filename) as List<PlurkItemViewModel>;
                    if (list != null && list.Any())
                    {
                        Items = new AdditiveBindableCollection<PlurkItemViewModel>(list);
                        _isCachedItemsLoaded = true;
                    }
                }
            }
        }

        #endregion

        private void SubscribeTimelineScroll(DependencyObject uiView)
        {
            var timeline = uiView.FindVisualChildByName<TimelineControl>("Timeline");
            var sv = timeline.GetVisualChildren<LongListSelector>()
                .FirstOrDefault();
            if (sv == null)
            {
                return;
            }
            _longListSelector = sv;
            var sb = sv.GetVisualDescendents<ScrollBar>(false).FirstOrDefault();
            sb.ValueChanged += (sender, args) =>
            {
                var offset = 100;
                if (!IsHasMore || args.NewValue < offset)
                {
                    return;
                }

                var isBottom = args.NewValue + offset >= sb.Maximum;
                if (isBottom && GetIsInfiniteScroll())
                {
                    RequestMore();
                }
            };
        }

        private bool GetIsInfiniteScroll()
        {
            var settings = IoC.Get<SettingsService>();
            return settings.GetIsInfiniteScroll();
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
                if (RequestMoreHandler != null)
                {
                    InternalRequest(RequestMoreHandler(_lastResult), false);
                }
                else
                {
                    IsHasMore = false;
                }
            }
        }

        private void InternalRequest(IObservable<TSource> observable, bool clear, SpecialFallback<TSource> specialFallback = null)
        {
            if (_requestHandler != null)
            {
                _requestHandler.Dispose();
            }
            Action<ICollection<PlurkItemViewModel>> cache = cacheItems =>
            {
                if (clear) CacheItems(cacheItems); // cache when clearing
            };

            Message = string.Empty;
            var tempIsHasMore = IsHasMore;
            IsHasMore = false;
            if (clear && !_isCachedItemsLoaded)
            {
                Clear();
            }

            _requestHandler = observable
                .DoProgress(ProgressService, ProgressMessage)
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
                        var prgId = ProgressService.Show(AppResources.msgUpdatingTimeline);
                        var items = MapUserPlurkToPlurkItemViewModel(result, plurks);
                        if (_isCachedItemsLoaded)
                        {
                            var collection = new AdditiveBindableCollection<PlurkItemViewModel>(items);
                            Execute.OnUIThread(() => Items = collection); // Fix LongList behavior
                            _isCachedItemsLoaded = false;
                            cache(collection);
                        }
                        else
                        {
                            Items.AddRange(items);
                            cache(Items);
                        }

                        if (IsHasMoreHandler != null)
                        {
                            IsHasMore = IsHasMoreHandler(plurks);
                        }

                        ProgressService.Hide(prgId);
                    }
                }, () =>
                {
                    RequestMoreForScroll();
                    OnRequestCompleted(_lastResult);
                });
        }

        private void CacheItems(IEnumerable<PlurkItemViewModel> items)
        {
            if (!EnableCaching)
            {
                return;
            }
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

        private void RequestMoreForScroll()
        {
            if (Items != null &&
                Items.Count < DefaultConfiguration.RequestItemsLimit &&
                GetIsInfiniteScroll())
            {
                RequestMore();
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
                ContextMenuEnabled = PlurkService.IsLoaded && !IsCompareIdInsteadOfPlurkId, // response page
                EnableHyperlink = this.EnableHyperlink,
                IsReplurkable = plurk.Plurk.Replurkable,
                IsReplurked = (plurk.Plurk.Replurked == true),
                ReplurkerName = getReplurkerName(plurk),
            });
        }

        public void CancelRequest()
        {
            ProgressService.Hide();
            _requestHandler.Dispose();
        }

        public void ScrollToTop()
        {
            if (_longListSelector != null && _longListSelector.ItemsSource.Count > 0)
            {
                _longListSelector.ScrollTo(_longListSelector.ItemsSource[0]);
            }
        }

        public void ScrollToEnd()
        {
            if (_longListSelector != null && _longListSelector.ItemsSource.Count > 0)
            {
                _longListSelector.ScrollTo(_longListSelector.ItemsSource[_longListSelector.ItemsSource.Count - 1]);
            }
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
