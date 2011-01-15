using System;
using System.Windows.Input;
using Caliburn.Micro;
using MetroPlurk.Services;
using MetroPlurk.Views;

namespace MetroPlurk.ViewModels
{
    public sealed class MainPageViewModel : LoginPivotViewModel, ISearchPage
    {
        private readonly INavigationService _navigationService;
        private readonly IPlurkService _plurkService;
        private readonly SearchResultViewModel _searchResult;
        private readonly SearchRecordsViewModel _searchRecords;
        private MainPage _view;
        
        private string _searchField;

        [SurviveTombstone]
        public string SearchField
        {
            get { return _searchField; }
            set
            {
                if (_searchField == value) return;
                _searchField = value;
                NotifyOfPropertyChange(() => SearchField);
            }
        }

        private string _loginButtonText;

        public string LoginButtonText
        {
            get { return _loginButtonText; }
            set
            {
                if (_loginButtonText == value) return;
                _loginButtonText = value;
                NotifyOfPropertyChange(() => LoginButtonText);
            }
        }

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

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            
            _view = view as MainPage;
            
            if (SearchField == null)
            {
                SearchField = "";
            }
            _searchResult.Search(SearchField);
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