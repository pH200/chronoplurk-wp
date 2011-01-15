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
        public SearchResultViewModel(IProgressService progressService, IPlurkService plurkService)
            : base(progressService, plurkService, "Searching")
        {
            this.DisplayName = "search";
            IsHasMoreHandler = plurks => plurks.HasMore;
        }

        public void Search(string query, int? offset = null)
        {
            var getPlurks = SearchCommand.Find(query, offset, PlurkService.Cookie).LoadAsync();
            RequestMoreHandler = plurks => SearchCommand.Find(query, plurks.LastOffset, PlurkService.Cookie).LoadAsync();

            Request(getPlurks);
        }
    }
}
