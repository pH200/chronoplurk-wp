using System;
using Caliburn.Micro;

namespace MetroPlurk.Services
{
    public static class NavigationExtensions
    {
        public static bool Navigate(this INavigationService navigationService, PlurkLocation location)
        {
            return navigationService.Navigate(location.ToUri());
        }
    }
}