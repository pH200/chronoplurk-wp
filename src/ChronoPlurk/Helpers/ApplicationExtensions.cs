using System.Windows;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Helpers
{
    public static class ApplicationExtensions
    {
        public static PhoneApplicationPage GetActivePage(this Application application)
        {
            if (application != null)
            {
                var rootVisual = application.RootVisual as PhoneApplicationFrame;
                if (rootVisual != null)
                {
                    return rootVisual.Content as PhoneApplicationPage;
                }
            }
            return null;
        }
    }
}
