using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
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

        public Func<bool> OnViewBackKeyPress { get; set; }

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

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (OnViewBackKeyPress != null)
            {
                e.Cancel = OnViewBackKeyPress();
            }
            if (!e.Cancel)
            {
                base.OnBackKeyPress(e);
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                OnViewBackKeyPress = null; // Dispose when page is released.
            }
            base.OnNavigatedFrom(e);
        }

        public AppBarMenuItem MuteButton { get; private set; }

        public AppBarMenuItem LikeButton { get; private set; }

        public AppBarButton ReplyButton { get; private set; }
        
        public AppBarButton EmoticonButton { get; private set; }

        public AppBarButton PhotosButton { get; private set; }

        public static readonly Uri ReplyIconUri = new Uri("Resources/Icons/appbar.edit.rest.png", UriKind.Relative);
        public static readonly Uri CheckIconUri = new Uri("Resources/Icons/appbar.check.rest.png", UriKind.Relative);

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar()
            {
                ForegroundColor = Colors.White,
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
            var emoticonButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.emoticon.png", UriKind.Relative),
                Text = AppResources.appbarEmoticon,
                IsEnabled = false,
                Message = "EmoticonAppBar"
            };
            EmoticonButton = emoticonButton;
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
            ApplicationBar.Buttons.Add(emoticonButton);
            ApplicationBar.Buttons.Add(photosButton);
            ApplicationBar.MenuItems.Add(likeButton);
            ApplicationBar.MenuItems.Add(muteButton);
        }

        public void ReplyRowAuto()
        {
            ReplyRow.Height = GridLength.Auto;
            DetailRow.Height = new GridLength(1.0, GridUnitType.Star);
        }

        public void ReplyRowMax()
        {
            DetailRow.Height = new GridLength(0);
            ReplyRow.Height = new GridLength(1.0, GridUnitType.Star);
        }
    }
}
