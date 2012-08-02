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
using ChronoPlurk.Views.Settings;

namespace ChronoPlurk.ViewModels.Settings
{
    public class SettingsBgViewModel : Screen, IChildT<SettingsPageViewModel>
    {
        public void SliderValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {

        }

        public void SliderStarted(object view)
        {
            ChangeHitTest(true);
        }

        private void ChangeHitTest(bool value)
        {
            var uiView = this.GetParent().GetView() as SettingsPage;
            if (uiView != null)
            {
                uiView.SettingsPivot.IsLocked = value;
            }
        }

        public void SliderCompleted(object view)
        {
            ChangeHitTest(false);
        }
    }
}
