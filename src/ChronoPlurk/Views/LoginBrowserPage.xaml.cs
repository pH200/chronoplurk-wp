using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Navigation;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using HtmlAgilityPack;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Views
{
    public partial class LoginBrowserPage
    {
        private static readonly string[] AllowedUrls = new string[]
        {
            "/m/login",
            "/m/authorize",
            "/login/authorize", // authorize* wildcard
            "/OAuth/authorize",
            "/OAuth/authorizeDone",
        };

        public LoginBrowserPage()
        {
            InitializeComponent();
            // AnimationContext = LayoutRoot;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string deviceName;
            if (!NavigationContext.QueryString.TryGetValue("DeviceName", out deviceName)
                || deviceName.IsNullOrEmpty())
            {
                deviceName = null;
            }

            var plurkService = IoC.Get<IPlurkService>();
            plurkService.GetRequestToken(deviceName)
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

        private void Browser_Navigating(object sender, NavigatingEventArgs e)
        {
            if (LoadingGrid.Visibility != Visibility.Visible)
            {
                var progressService = IoC.Get<IProgressService>();
                progressService.Show();
            }
            var path = e.Uri.AbsolutePath;
            if (AllowedUrls.All(url => !path.StartsWith(url)))
            {
                e.Cancel = true;
                NavigationService.GoBack();
            }
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            var progressService = IoC.Get<IProgressService>();
            progressService.Hide();
            if (LoadingGrid.Visibility == Visibility.Visible)
            {
                loadedStoryboard.Begin();
            }
            if (e.Uri.OriginalString.Contains("authorizeDone"))
            {
                ProcessVerifier();
                NavigationService.GoBack();
            }
        }

        private void ProcessVerifier()
        {
            // verifierText = Browser.InvokeScript("eval", "document.getElementById('oauth_verifier').textContent") as string;
            var document = new HtmlDocument();
            document.LoadHtml(Browser.SaveToString());
            var element = document.GetElementbyId("oauth_verifier");
            if (element != null && element.InnerText != null)
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