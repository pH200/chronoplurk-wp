using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public event EventHandler<VisualStateChangedEventArgs> VerticalCompressionChanged;

        private void LongListSelector_Loaded(object sender, RoutedEventArgs e)
        {
            //get TemplatedListBox inside LongListSelector
            var tlb = (FrameworkElement) VisualTreeHelper.GetChild(Items, 0);
            //get ScrollViewer inside TemplatedListBox
            var sv = (FrameworkElement) VisualTreeHelper.GetChild(tlb, 0);
            //MS says VisualGroups are inside first Child of ScrollViewer 
            var here = (FrameworkElement) VisualTreeHelper.GetChild(sv, 0);
            var groups = VisualStateManager.GetVisualStateGroups(here);
            var vc = groups.Cast<VisualStateGroup>().First(g => g.Name == "VerticalCompression");
            vc.CurrentStateChanged += new EventHandler<VisualStateChangedEventArgs>(vc_CurrentStateChanged);
        }

        private void vc_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            var handler = VerticalCompressionChanged;
            if (handler != null) handler(this, e);
        }
    }
}
