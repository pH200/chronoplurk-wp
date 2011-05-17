using System;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using MetroPlurk.Services;
using MetroPlurk.Views;
using NotifyPropertyWeaver;

namespace MetroPlurk.ViewModels
{
    [NotifyForAll]
    public sealed class SearchPageViewModel : PlurkAppBarPage, ISearchPage
    {
        private readonly IProgressService _progressService;
        private readonly SearchResultViewModel _searchResult;
        private readonly SearchRecordsViewModel _searchRecords;
        private SearchPage _view;

        public string SearchField { get; set; }

        public SearchPageViewModel(
            INavigationService navigationService,
            IPlurkService plurkService,
            LoginViewModel loginViewModel,
            IProgressService progressService,
            SearchResultViewModel searchResult,
            SearchRecordsViewModel searchRecords)
            : base(navigationService, plurkService, loginViewModel)
        {
            _progressService = progressService;
            _searchResult = searchResult;
            _searchRecords = searchRecords;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.Add(_searchResult);
            Items.Add(_searchRecords);
            ActivateItem(_searchResult);
        }
        
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (SearchPage)view;
            SearchOnLoaded();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            _progressService.Hide();
        }

        private void SearchOnLoaded()
        {
            if (_searchResult.Items.IsEmpty() &&
                SearchField != null &&
                SearchField.Trim() != "")
            {
                Search();
            }
            else
            {
                _view.SearchField.Focus();
            }
        }

        public void Search()
        {
            if (SearchField == null)
            {
                SearchField = string.Empty;
            }
            if (_view != null)
            {
                _view.Focus();
            }

            var query = SearchField.Trim();
            if (!String.IsNullOrEmpty(query))
            {
                _searchRecords.Add(query);
            }
            ActivateItem(_searchResult);
            _searchResult.Search(query);
        }

        public void Search(string query)
        {
            SearchField = query;
            Search();
        }

        public void SearchKey(KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                Search();
            }
        }
    }
}