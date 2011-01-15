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

namespace MetroPlurk
{
    public partial class TimelineControl : UserControl
    {
        public TimelineControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler RequestMoreClick;

        public void OnRequestMoreClick(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler handler = RequestMoreClick;
            if (handler != null) handler(this, e);
        }

        private void RequestMore_Click(object sender, RoutedEventArgs e)
        {
            OnRequestMoreClick(sender, e);
        }
    }
}
