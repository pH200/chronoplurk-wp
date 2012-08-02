using System;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Plurto.Commands;
using Plurto.Core;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels
{
    public sealed class SearchResultViewModel : TimelineBaseViewModel<SearchResult>, IChildT<ISearchPage>
    {
        private Type _lastParent;

        public SearchResultViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService)
            : base(navigationService, progressService, plurkService, plurkContentStorageService)
        {
            this.DisplayName = AppResources.search;
            IsHasMoreHandler = plurks => plurks.HasMore;
            ProgressMessage = AppResources.msgSearching;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (_lastParent != null && // Has navigated from ISearchPage
                _lastParent == typeof(MainPageViewModel) && // Has navigated from initial search page
                !(Parent is MainPageViewModel)) // Isn't navigated to initial search page
            {
                this.Clear();
            }
        }

        protected override void OnDeactivate(bool close)
        {
            _lastParent = Parent.GetType();

            base.OnDeactivate(close);
        }

        public void Search(string query, int? offset = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                PlurkTop();
                return;
            }
            var getPlurks = SearchCommand.PlurkSearch(query, offset).Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks => SearchCommand.PlurkSearch(query, plurks.LastOffset).Client(PlurkService.Client).ToObservable();

            Request(getPlurks);
            
            ScrollToTop();
        }

        public void PlurkTop()
        {
            var culture = Plurto.Helpers.Culture.GetRecommendPlurkCulture();
            PlurkTop(culture.CollectionName, sorting:PlurkTopSorting.New);
        }

        public void PlurkTop(string collectionName, int? filter=null, double? offset=null, PlurkTopSorting? sorting=null)
        {
            var getPlurks = PlurkTopCommand.GetPlurks(collectionName, offset, 30, sorting, filter)
                .Client(PlurkService.Client)
                .ToObservable()
                .Select(s => new SearchResult()
                {
                    HasMore = false,
                    LastOffset = null,
                    Plurks = s.Plurks,
                    Users = s.Users,
                });
            RequestMoreHandler = null;

            Request(getPlurks);

            ScrollToTop();
        }
    }
}
