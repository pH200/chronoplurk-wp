using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using Microsoft.Phone.Shell;
using WP7Contrib.View.Transitions.Animation;

namespace ChronoPlurk.Views.Compose
{
    public partial class ComposePage : ISwitchControl
    {
        public ComposePage()
        {
            InitializeComponent();
            //AnimationContext = LayoutRoot;
            BuildAppBar();
        }

        //protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        //{
        //    switch (animationType)
        //    {
        //        case AnimationType.NavigateForwardOut:
        //        case AnimationType.NavigateBackwardOut:
        //            return new SlideDownAnimator() { RootElement = LayoutRoot };
        //        case AnimationType.NavigateForwardIn:
        //            return new SlideUpAnimator() { RootElement = LayoutRoot };
        //    }
        //    return base.GetAnimation(animationType, toOrFrom);
        //}

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar()
            {
                ForegroundColor = Colors.White,
                BackgroundColor = PlurkResources.PlurkColor
            };
            var plurkButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.check.rest.png", UriKind.Relative),
                Text = AppResources.appbarPlurk,
                Message = "PlurkAppBar"
            };
            PlurkButton = plurkButton;
            var emoticonButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.emoticon.png", UriKind.Relative),
                Text = AppResources.appbarEmoticon,
                Message = "EmoticonAppBar"
            };
            var photosButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.feature.camera.rest.png", UriKind.Relative),
                Text = AppResources.appbarPhotos,
                Message = "PhotosAppBar"
            };
            PhotosButton = photosButton;
            var privateButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.lock.png", UriKind.Relative),
                Text = AppResources.appbarPrivate,
                Message = "PrivateAppBar"
            };
            ApplicationBar.Buttons.Add(plurkButton);
            ApplicationBar.Buttons.Add(emoticonButton);
            ApplicationBar.Buttons.Add(photosButton);
            ApplicationBar.Buttons.Add(privateButton);
        }

        protected AppBarButton PlurkButton { get; set; }

        protected AppBarButton PhotosButton { get; set; }

        public void Switch(bool enable)
        {
            PlurkButton.IsEnabled = enable;
            PhotosButton.IsEnabled = enable;
        }
    }
}
