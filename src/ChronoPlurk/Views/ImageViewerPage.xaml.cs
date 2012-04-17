using System;
using System.Windows.Media;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using WP7Contrib.View.Transitions.Animation;

namespace ChronoPlurk.Views
{
    public partial class ImageViewerPage
    {
        public Uri ImageUri { get; set; }

        public ImageViewerPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            BuildAppBar();

            ImageUri = new Uri("about:blank", UriKind.Absolute);

            Browser.Navigating += (sender, args) =>
            {
                BrowserProgress.IsIndeterminate = true;
            };
            Browser.Navigated += (sender, args) =>
            {
                BrowserProgress.IsIndeterminate = false;
            };
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            switch (animationType)
            {
                case AnimationType.NavigateForwardOut:
                case AnimationType.NavigateBackwardOut:
                    return new SlideDownAnimator() { RootElement = LayoutRoot };
                case AnimationType.NavigateForwardIn:
                case AnimationType.NavigateBackwardIn:
                    return new SlideUpAnimator() { RootElement = LayoutRoot };
            }
            return base.GetAnimation(animationType, toOrFrom);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string uriString;
            if (NavigationContext.QueryString.TryGetValue("ImageUri", out uriString))
            {
                try
                {
                    ImageUri = new Uri(uriString, UriKind.Absolute);
                    UrlTextBox.Text = uriString;
                    Browser.Navigate(ImageUri);
                }
                catch (UriFormatException)
                {
                }
            }
            base.OnNavigatedTo(e);
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar()
            {
                ForegroundColor = Colors.White,
                BackgroundColor = PlurkResources.PlurkColor,
                Mode = ApplicationBarMode.Minimized,
            };
            var refreshButton = new ApplicationBarIconButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.refresh.rest.png", UriKind.Relative),
                Text = AppResources.appbarRefresh,
            };
            refreshButton.Click += (sender, args) =>
            {
                // Browser.InvokeScript("eval", "history.go()"); // Crashes
                Browser.Navigate(ImageUri);
            };
            var openInIeButton = new ApplicationBarMenuItem()
            {
                Text = AppResources.appbarOpenInIE
            };
            openInIeButton.Click += (sender, args) =>
            {
                var task = new WebBrowserTask()
                {
                    Uri = ImageUri
                };
                task.Show();
            };
            ApplicationBar.Buttons.Add(refreshButton);
            ApplicationBar.MenuItems.Add(openInIeButton);
        }
    }
}