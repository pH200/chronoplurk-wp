using System;
using Caliburn.Micro;
using ChronoPlurk.Services;

namespace ChronoPlurk.ViewModels
{
    public class PlurkAppBarPage : LoginPivotViewModel
    {
        protected readonly INavigationService NavigationService;
        protected readonly IPlurkService PlurkService;

        public PlurkAppBarPage(
            INavigationService navigationService,
            IPlurkService plurkService,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            NavigationService = navigationService;
            PlurkService = plurkService;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (!PlurkService.IsLoaded)
            {
                NavigationService.GotoLoginPage();
            }
        }

        public void ComposeAppBar()
        {
            NavigationService.Navigate(new Uri("/Views/ComposePage.xaml", UriKind.Relative));
        }

        public void SearchAppBar()
        {
            NavigationService.Navigate(new Uri("/Views/SearchPage.xaml", UriKind.Relative));
        }

        public void SettingsAppBar()
        {
            NavigationService.Navigate(new Uri("/Views/Settings/SettingsPage.xaml", UriKind.Relative));
        }

        public void ProfileAppBar()
        {
            NavigationService.Navigate(new Uri("/Views/ProfilePage.xaml", UriKind.Relative));
        }
    }
}