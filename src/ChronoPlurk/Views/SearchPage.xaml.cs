﻿using System;
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
    public partial class SearchPage
    {
        public SearchPage()
        {
            InitializeComponent();
            BuildAppBar();
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar()
            {
                ForegroundColor = Colors.White,
                BackgroundColor = PlurkResources.PlurkColor
            };
            var searchButton = new AppBarButton()
            {
                IconUri = new Uri("Resources/Icons/appbar.feature.search.rest.png", UriKind.Relative),
                Text = AppResources.appbarSearch,
                Message = "StartSearchAppBar"
            };

            ApplicationBar.Buttons.Add(searchButton);
        }
    }
}
