using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Caliburn.Micro;
using ChronoPlurk.Services;

namespace ChronoPlurk
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

        internal struct NavigationHelper
        {
            public Uri TargetUri { get; set; }
            public Uri CurrentUri { get; set; }
            public NavigationMode NavMode { get; set; }
        }
    }
}
