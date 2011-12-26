using System;
using Caliburn.Micro;

namespace ChronoPlurk.Services
{
    public static class NavigationExtensions
    {
        public static bool RemoveBackEntryFlag { get; private set; }

        public static bool Navigate(this INavigationService navigationService, PlurkLocation location)
        {
            return navigationService.Navigate(location.ToUri());
        }

        public static void GotoLoginPage(this INavigationService navigationService, bool redirectMainPage=false)
        {
            const string pageUrl = "//Views/LoginPage.xaml";
            navigationService.Navigate(new Uri(pageUrl + "?RedirectMainPage=" + redirectMainPage, UriKind.Relative));
        }

        public static void SetRemoveBackEntryFlag(this INavigationService navigationService)
        {
            RemoveBackEntryFlag = true;
        }

        public static void UseRemoveBackEntryFlag(this INavigationService navigationService)
        {
            if (RemoveBackEntryFlag)
            {
                if (navigationService.CanGoBack)
                {
                    navigationService.RemoveBackEntry();
                }
                RemoveBackEntryFlag = false;
            }
        }
    }
}
