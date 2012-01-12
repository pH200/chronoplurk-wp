using System;
using System.Linq;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Plurto.Commands;
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
            var getPlurks = SearchCommand.Find(query, offset).Client(PlurkService.Client).ToObservable();
            RequestMoreHandler = plurks => SearchCommand.Find(query, plurks.LastOffset).Client(PlurkService.Client).ToObservable();

            Request(getPlurks);
            
            ScrollToTop();
        }
    }
}
