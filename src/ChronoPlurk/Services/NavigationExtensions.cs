using System;
using Caliburn.Micro;

namespace ChronoPlurk.Services
{
    public static class NavigationExtensions
    {
        public static bool Navigate(this INavigationService navigationService, PlurkLocation location)
        {
            return navigationService.Navigate(location.ToUri());
        }
    }
}