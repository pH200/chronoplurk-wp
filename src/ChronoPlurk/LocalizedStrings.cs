using ChronoPlurk.Resources.i18n;

namespace ChronoPlurk
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}
