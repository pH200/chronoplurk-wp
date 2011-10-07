using System;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.Views;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public sealed class MainPageViewModel : LoginPivotViewModel, ISearchPage
    {
        private readonly INavigationService _navigationService;
        private readonly IPlurkService _plurkService;
        private readonly SearchResultViewModel _searchResult;
        private readonly SearchRecordsViewModel _searchRecords;
        private MainPage _view;

        public string SearchField { get; set; }

        public string LoginButtonText { get; set; }

        public MainPageViewModel(
            INavigationService navigationService,
            IPlurkService plurkService,
            SearchResultViewModel searchResult,
            SearchRecordsViewModel searchRecords,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            _navigationService = navigationService;
            _plurkService = plurkService;
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

        protected override void OnActivate()
        {
            base.OnActivate();

            SetupLoginButton();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (MainPage) view;

            if (SearchField == null)
            {
                SearchField = "";
            }
            if (_searchResult.Items.IsNullOrEmpty())
            {
                _searchResult.Search(SearchField);
            }
        }

        private void SetupLoginButton()
        {
            LoginButtonText = _plurkService.IsLoaded ? "to main page" : "login";
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
        
        public void Login()
        {
            _searchResult.CancelRequest();
            if (!_plurkService.IsLoaded)
            {
                ShowLoginPopup(false, new Uri("/Views/PlurkMainPage.xaml", UriKind.Relative));
            }
            else
            {
                _navigationService.Navigate(new Uri("/Views/PlurkMainPage.xaml", UriKind.Relative));
            }
        }
    }
}