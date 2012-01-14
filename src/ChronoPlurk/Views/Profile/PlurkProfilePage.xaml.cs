using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WP7Contrib.View.Transitions.Animation;

namespace ChronoPlurk.Views.Profile
{
    public partial class PlurkProfilePage
    {
        public PlurkProfilePage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            BuildAppBar();
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            switch (animationType)
            {
                case AnimationType.NavigateForwardOut:
                    return new DefaultPageAnimator() { RootElement = LayoutRoot };
                case AnimationType.NavigateBackwardIn:
                    return new DefaultPageAnimator() { RootElement = LayoutRoot };
            }
            return base.GetAnimation(animationType, toOrFrom);
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar()
            {
                BackgroundColor = PlurkResources.PlurkColor
            };
            var refreshButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.refresh.rest.png", UriKind.Relative),
                Text = AppResources.appbarRefresh,
                Message = "RefreshAppBar"
            };

            ApplicationBar.Buttons.Add(refreshButton);
        }
    }
}
