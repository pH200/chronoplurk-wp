using System;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;

namespace ChronoPlurk.Services
{
    public static class NavigationExtensions
    {
        public static bool RemoveBackEntryFlag { get; private set; }
        
        public static bool RemoveAllBackEntriesFlag { get; private set; }

        public static bool Navigate(this INavigationService navigationService, PlurkLocation location)
        {
            return navigationService.Navigate(location.ToUri());
        }

        public static void GotoLoginPage(this INavigationService navigationService, bool redirectMainPage=false)
        {
            const string pageUrl = "//Views/LoginPage.xaml";
            navigationService.Navigate(new Uri(pageUrl + "?RedirectMainPage=" + redirectMainPage, UriKind.Relative));
        }

        public static void GotoMainPage(this INavigationService navigationService, bool removeAllBackEntries=true)
        {
            const string pageUrl = "//Views/MainPage.xaml";
            if (removeAllBackEntries)
            {
                navigationService.SetRemoveAllBackEntriesFlag();
            }
            navigationService.Navigate(new Uri(pageUrl, UriKind.Relative));
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

        public static void SetRemoveAllBackEntriesFlag(this INavigationService navigationService)
        {
            RemoveAllBackEntriesFlag = true;
        }

        public static void UseRemoveAllBackEntriesFlag(this INavigationService navigationService, Page page)
        {
            if (RemoveAllBackEntriesFlag && page != null)
            {
                var ns = page.NavigationService;
                var backStackCount = ns.BackStack.Count();
                for (var i = 0; i < backStackCount; i++)
                {
                    if (ns.CanGoBack)
                    {
                        ns.RemoveBackEntry();
                    }
                }
            }
        }
    }
}
