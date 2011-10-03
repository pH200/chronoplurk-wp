using System.Windows;
using System.Windows.Controls;

namespace ChronoPlurk.Views.PlurkControls
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
