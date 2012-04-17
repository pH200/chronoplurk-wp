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

        public static void GotoProfilePage(this INavigate navigationService, int userId, string username, string avatar)
        {
            const string pageUrl = "//Views/Profile/PlurkProfilePage.xaml";
            var uriString = pageUrl + String.Format("?UserId={0}&Username={1}&UserAvatar={2}", userId, username, avatar);
            var uri = new Uri(uriString, UriKind.Relative);
            navigationService.Navigate(uri);
        }

        public static void GotoComposePage(this INavigate navigationService)
        {
            const string pageUrl = "//Views/Compose/ComposePage.xaml";
            navigationService.Navigate(new Uri(pageUrl, UriKind.Relative));
        }

        public static void GotoImageBrowserPage(this INavigationService navigationService, Uri uri)
        {
            const string pageUrl = "//Views/ImageViewerPage.xaml?ImageUri=";
            navigationService.Navigate(new Uri(pageUrl + uri, UriKind.Relative));
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
                var service = IoC.Get<PlurkHolderService>();
                service.Clear();
            }
        }
    }
}
