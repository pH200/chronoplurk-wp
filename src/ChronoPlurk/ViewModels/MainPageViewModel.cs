using System;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Views;
using Microsoft.Phone.Tasks;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public sealed class MainPageViewModel : PivotViewModel, ISearchPage
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
            SearchRecordsViewModel searchRecords)
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
            LoginButtonText = _plurkService.IsLoaded ? AppResources.toMainPage : AppResources.login;
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
                _navigationService.GotoLoginPage(redirectMainPage:true);
            }
            else
            {
                _navigationService.Navigate(new Uri("/Views/PlurkMainPage.xaml", UriKind.Relative));
            }
        }

        public void SignUp()
        {
            var uri = new Uri("http://www.plurk.com/Users/showRegister", UriKind.Absolute);
            var webBrowserTask = new WebBrowserTask() { Uri = uri };
            webBrowserTask.Show();
        }
    }
}
