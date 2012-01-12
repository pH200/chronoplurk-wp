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

namespace ChronoPlurk.Views.Compose
{
    public partial class FriendsSelectionPage : PhoneApplicationPage
    {
        public FriendsSelectionPage()
        {
            InitializeComponent();
            BuildAppBar();
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar()
            {
                BackgroundColor = PlurkResources.PlurkColor
            };
            var downloadButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.download.rest.png", UriKind.Relative),
                Text = AppResources.appbarDownloadFriendList,
                Message = "DownloadAppBar"
            };
            var completeButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.check.rest.png", UriKind.Relative),
                Text = AppResources.appbarComplete,
                Message = "CompleteAppBar"
            };
            var clearButton = new AppBarMenuItem()
            {
                Text = AppResources.appbarClearSelection,
                Message = "ClearAppBar"
            };
            var helpButton = new AppBarMenuItem()
            {
                Text = AppResources.appbarHelp,
                Message = "HelpAppBar"
            };

            ApplicationBar.Buttons.Add(downloadButton);
            ApplicationBar.Buttons.Add(completeButton);
            ApplicationBar.MenuItems.Add(clearButton);
            ApplicationBar.MenuItems.Add(helpButton);
        }
    }
}
