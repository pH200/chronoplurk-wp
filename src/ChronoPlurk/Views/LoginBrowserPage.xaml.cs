using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Navigation;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using HtmlAgilityPack;

namespace ChronoPlurk.Views
{
    public partial class LoginBrowserPage
    {
        public LoginBrowserPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var plurkService = IoC.Get<IPlurkService>();
            plurkService.GetRequestToken()
                .ObserveOnDispatcher()
                .Catch<Uri, Exception>(ex =>
                {
                    Execute.OnUIThread(() =>
                    {
                        MessageBox.Show(AppResources.unhandledErrorMessage.Replace("\\n", Environment.NewLine));
                        NavigationService.GoBack();
                    });
                    return Observable.Empty<Uri>();
                })
                .Subscribe(uri =>
                {
                    Browser.Source = uri;
                });
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            if (LoadingGrid.Visibility == Visibility.Visible)
            {
                loadedStoryboard.Begin();
            }
            if (e.Uri.ToString().Contains("authorizeDone"))
            {
                ProcessVerifier();
                NavigationService.GoBack();
            }
        }

        private void ProcessVerifier()
        {
            var document = new HtmlDocument();
            document.LoadHtml(Browser.SaveToString());
            var element = document.GetElementbyId("oauth_verifier");
            if(element.InnerText != null)
            {
                var verifier = element.InnerText.Trim();

                var plurkService = IoC.Get<IPlurkService>();
                plurkService.VerifierTemp = verifier;
            }
        }

        private void loadedStoryboard_Completed(object sender, EventArgs e)
        {
            LoadingGrid.Visibility = Visibility.Collapsed;
        }
    }
}