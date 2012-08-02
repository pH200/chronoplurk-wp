using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels.Core;
using ChronoPlurk.Views;
using ChronoPlurk.Views.Compose;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Core
{
    public class SpecialFrameAdapter : FrameAdapter
    {
        public SpecialFrameAdapter(Frame frame, bool treatViewAsLoaded = false)
            : base(frame, treatViewAsLoaded)
        {
        }

        protected override void TryInjectQueryString(object viewModel, Page page)
        {
            var viewModelType = viewModel.GetType();

            if (typeof(INavigationInjectionRedirect).IsAssignableFrom(viewModelType))
            {
                var redirectedViewModel = ((INavigationInjectionRedirect) viewModel).GetRedirectedViewModel();
                TryInjectQueryString(redirectedViewModel, page);
            }
            else
            {
                base.TryInjectQueryString(viewModel, page);
            }
        }

        protected override void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                var page = CurrentContent as IPlurkHolder;
                if (page != null)
                {
                    var service = IoC.Get<PlurkHolderService>();
                    service.RemoveAll(page);
                }
            }
            base.OnNavigating(sender, e);
        }

        protected override void OnNavigated(object sender, NavigationEventArgs e)
        {
            var page = e.Content as PhoneApplicationPage;
            SetOrientationInternal(page);
            SetBackgroundInternal(page);
            ProcessMainPageBackEntryRemoval(page);
            base.OnNavigated(sender, e);
        }


        private static void SetBackgroundInternal(PhoneApplicationPage page)
        {
            if (page is ImageViewerPage || page is ListPickerPage)
            {
                return;
            }
            var service = IoC.Get<BackgroundImageService>();
            if (service != null)
            {
                service.ApplyBackground(page);
            }
        }

        private void ProcessMainPageBackEntryRemoval(PhoneApplicationPage page)
        {
            if (page is MainPage)
            {
                this.UseRemoveAllBackEntriesFlag(page);
            }
        }

        private void SetOrientationInternal(PhoneApplicationPage page)
        {
            if (page != null)
            {
                var autoRotateSerivce = IoC.Get<AutoRotateService>();
                if (autoRotateSerivce != null)
                {
                    SetPageSupportedOrientation(autoRotateSerivce, page);
                }
            }
        }

        public static void SetPageSupportedOrientation(AutoRotateService autoRotateSerivce, PhoneApplicationPage page)
        {
            if (page is ImageViewerPage) // Ignore settings
            {
                page.SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
            }
            else if (page is ComposePage || page is ListPickerPage) // Compose pages
            {
                page.SupportedOrientations = autoRotateSerivce.ComposePageOrientation;
            }
            else // Settings
            {
                page.SupportedOrientations = autoRotateSerivce.PageOrientation;
            }
        }

        internal struct NavigationHelper
        {
            public Uri TargetUri { get; set; }
            public Uri CurrentUri { get; set; }
            public NavigationMode NavMode { get; set; }
        }
    }
}
