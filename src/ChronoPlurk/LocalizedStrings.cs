using System.Globalization;
using System.Threading;
using ChronoPlurk.Resources.i18n;

namespace ChronoPlurk
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        static LocalizedStrings()
        {
            Culture = Thread.CurrentThread.CurrentCulture;
            UICulture = Thread.CurrentThread.CurrentUICulture;
        }

        public static CultureInfo Culture { get; set; }

        public static CultureInfo UICulture { get; set; }

        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}
