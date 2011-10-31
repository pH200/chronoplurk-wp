using System;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using ChronoPlurk.Views.Settings;

namespace ChronoPlurk.ViewModels.Settings
{
    public class SettingsOssCreditsPageViewModel : Screen
    {
        private static string HtmlString { get; set; }

        public SettingsOssCreditsPageViewModel()
        {
            if (HtmlString == null)
            {
                LoadCredtisHtmlString();
            }
        }

        private static void LoadCredtisHtmlString()
        {
            var resourceStream = Application.GetResourceStream(new Uri("/ChronoPlurk;component/Resources/credit.html", UriKind.Relative));
            using (var stream = resourceStream.Stream)
            {
                using (var streamReader = new StreamReader(stream))
                {
                    HtmlString = streamReader.ReadToEnd();
                }
            }
        }

        protected override void OnViewLoaded(object view)
        {
            var settingsView = view as SettingsOssCreditsPage;
            if (settingsView != null)
            {
                LoadCredits(settingsView);
            }
            base.OnViewLoaded(view);
        }

        private static void LoadCredits(SettingsOssCreditsPage settingsView)
        {
            settingsView.CreditsBrowser.NavigateToString(HtmlString);
        }
    }
}
