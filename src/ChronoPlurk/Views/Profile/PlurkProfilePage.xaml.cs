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

namespace ChronoPlurk.Views.Profile
{
    public partial class PlurkProfilePage : PhoneApplicationPage
    {
        public PlurkProfilePage()
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
