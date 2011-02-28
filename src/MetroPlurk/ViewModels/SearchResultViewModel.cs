using System;
using System.Linq;
using Caliburn.Micro;
using MetroPlurk.Services;
using Plurto.Commands;
using Plurto.Entities;

namespace MetroPlurk.ViewModels
{
    public sealed class SearchResultViewModel : TimelineBaseViewModel<SearchResult>
    {
        private Type _lastParent;

        public SearchResultViewModel
            (INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService)
            : base(navigationService, progressService, plurkService, "Searching")
        {
            this.DisplayName = "search";
            IsHasMoreHandler = plurks => plurks.HasMore;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (_lastParent != null &&
                _lastParent == typeof(MainPageViewModel) &&
                !(Parent is MainPageViewModel))
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
            var getPlurks = SearchCommand.Find(query, offset, PlurkService.Cookie).LoadAsync();
            RequestMoreHandler = plurks => SearchCommand.Find(query, plurks.LastOffset, PlurkService.Cookie).LoadAsync();

            Request(getPlurks);
            
            ScrollToTop();
        }
    }
}
