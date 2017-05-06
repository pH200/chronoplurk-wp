using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using WP7Contrib.View.Transitions.Animation;

namespace ChronoPlurk.Views
{
    public partial class ImageViewerPage
    {
        private const string EmbedPage = @"
<!doctype html>
<html>
<head>
<title>ChronoPlurk Image Browser</title>
<meta name=""viewport"" content=""width=800, initial-scale=1.0"">
<script>
function replaceImg(imgsrc) {
    var img = document.createElement('img');
    img.id = 'inner'
    img.src = imgsrc;
    img.addEventListener('load', function(e) {
        window.external.notify('load');
    });
    var container = document.getElementById('container');
    container.replaceChild(img, document.getElementById('inner'));
}
function changeBg(color) {
    document.body.bgColor = color;
}
</script>
<style>
body { margin: 20px 0; }
#container { text-align: center; }
#footer { height: 20px; }
</style>
</head>
<body bgcolor=""black"">
<div id=""container"">
    <div id=""inner""></div>
</div>
<div id=""footer""></div>
</body>
</html>
";
        private static readonly string[] SupportExts = new[] {".jpg", ".jpeg", ".png"};

        public Uri ImageUri { get; set; }

        public ImageViewerPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;

            ImageUri = new Uri("about:blank", UriKind.Absolute);
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

                    BuildAppBar();

                    Browser.NavigateToString(EmbedPage);
                }
                catch (UriFormatException)
                {
                }
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnOrientationChanged(Microsoft.Phone.Controls.OrientationChangedEventArgs e)
        {
            InvalidateOnOrientation(e.Orientation);
            base.OnOrientationChanged(e);
        }

        private void InvalidateOnOrientation(PageOrientation orientation)
        {
            if (orientation == PageOrientation.LandscapeRight) // Clockwise
            {
                ApplicationBar.IsVisible = false;
                VisualStateManager.GoToState(this, "LandscapeRight", false);
            }
            else if (orientation == PageOrientation.LandscapeLeft) // Counterclockwise
            {
                ApplicationBar.IsVisible = false;
                VisualStateManager.GoToState(this, "LandscapeLeft", false);
            }
            else
            {
                ApplicationBar.IsVisible = true;
                VisualStateManager.GoToState(this, "Portrait", true);
            }
        }

        private void Browser_Navigating(object sender, Microsoft.Phone.Controls.NavigatingEventArgs e)
        {
            ProgressBar.IsIndeterminate = true;
        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            LoadImage();
        }

        private void Browser_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            ProgressBar.IsIndeterminate = false;
        }

        private void LoadImage()
        {
            try
            {
                Browser.InvokeScript("replaceImg", ImageUri.ToString());
            }
            catch
            {
                // Ignore exception
            }
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
                Browser.NavigateToString(EmbedPage);
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

            if (IsSupportedImage())
            {
                AddDownloadButton();
            }
        }

        private void AddDownloadButton()
        {
            var downloadButton = new ApplicationBarMenuItem()
            {
                Text = AppResources.appbarSaveImage
            };
            downloadButton.Click += (sender, args) =>
            {
                downloadButton.IsEnabled = false;
                var progress = IoC.Get<IProgressService>();
                var prgId = progress.Show(AppResources.msgDownloadingImage);
                ProgressBar.IsIndeterminate = true;

                var client = WebRequest.Create(ImageUri);
                var ap = Observable.FromAsyncPattern<WebResponse>(client.BeginGetResponse, client.EndGetResponse)();

                Func<WebResponse, MemoryStream> onWebResponse = response =>
                {
                    using (response)
                    using (var responseStream = response.GetResponseStream())
                    {
                        var memoryStream = new MemoryStream();
                        responseStream.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        return memoryStream;
                    }
                };
                Action<MemoryStream> onImageStream = memoryStream =>
                {
                    var bitmap = new BitmapImage {CreateOptions = BitmapCreateOptions.None};
                    bitmap.SetSource(memoryStream);
                    var wb = new WriteableBitmap(bitmap);
                    memoryStream.Flush();
                    wb.SaveJpeg(memoryStream, wb.PixelWidth, wb.PixelHeight, 0, 90);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var library = new MediaLibrary();
                    library.SavePicture(Guid.NewGuid() + ".jpg", memoryStream);
                };
                Action<MemoryStream> afterSaved = memoryStream =>
                {
                    ProgressBar.IsIndeterminate = false;
                    downloadButton.IsEnabled = true;
                };

                var filename = ImageUri.AbsolutePath.Split('/').Last();
                ap.Select(onWebResponse)
                    .ObserveOnDispatcher()
                    .Do(onImageStream)
                    .DoProgress(progress, prgId)
                    .Do(afterSaved)
                    .Subscribe(memoryStream => ShowProgressMessage.Begin(),
                               err => MessageBox.Show(AppResources.msgErrImageDownloading,
                                                      filename,
                                                      MessageBoxButton.OK));
            };
            ApplicationBar.MenuItems.Add(downloadButton);
        }

        private bool IsSupportedImage()
        {
            return SupportExts.Any(ext => ImageUri.AbsolutePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}
