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

namespace ChronoPlurk.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }
    }
}
