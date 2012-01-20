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

namespace ChronoPlurk.Views
{
    public partial class PlurkDetailPage
    {
        public PlurkDetailPage()
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
                case AnimationType.NavigateBackwardOut:
                    return new SlideDownAnimator() { RootElement = LayoutRoot };
                case AnimationType.NavigateForwardIn:
                    return new SlideUpAnimator() { RootElement = LayoutRoot };
            }
            return base.GetAnimation(animationType, toOrFrom);
        }

        public AppBarMenuItem MuteButton { get; private set; }

        public AppBarMenuItem LikeButton { get; private set; }

        public AppBarButton ReplyButton { get; private set; }

        public AppBarButton PhotosButton { get; private set; }

        public static readonly Uri ReplyIconUri = new Uri("Resources/Icons/appbar.edit.rest.png", UriKind.Relative);
        public static readonly Uri CheckIconUri = new Uri("Resources/Icons/appbar.check.rest.png", UriKind.Relative);

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
            var replyButton = new AppBarButton()
            {
                IconUri = ReplyIconUri,
                Text = AppResources.appbarReply,
                Message = "ReplyAppBar"
            };
            ReplyButton = replyButton;
            var photosButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.feature.camera.rest.png", UriKind.Relative),
                Text = AppResources.appbarPhotos,
                IsEnabled = false,
                Message = "PhotosAppBar"
            };
            PhotosButton = photosButton;
            var likeButton = new AppBarMenuItem()
            {
                Text = AppResources.like,
                Message = "LikeAppBar"
            };
            LikeButton = likeButton;
            var muteButton = new AppBarMenuItem()
            {
                Text = AppResources.mute,
                Message = "MuteAppBar"
            };
            MuteButton = muteButton;

            ApplicationBar.Buttons.Add(refreshButton);
            ApplicationBar.Buttons.Add(replyButton);
            ApplicationBar.Buttons.Add(photosButton);
            ApplicationBar.MenuItems.Add(likeButton);
            ApplicationBar.MenuItems.Add(muteButton);
        }
    }
}
