using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.Views;
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

        protected override void OnNavigated(object sender, NavigationEventArgs e)
        {
            var page = e.Content as PhoneApplicationPage;
            SetOrientationInternal(page);
            ProcessMainPageBackEntryRemoval(page);
            base.OnNavigated(sender, e);
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
            if (page.Name.Contains("ComposePage"))
            {
                page.SupportedOrientations = autoRotateSerivce.ComposePageOrientation;
            }
            else
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
