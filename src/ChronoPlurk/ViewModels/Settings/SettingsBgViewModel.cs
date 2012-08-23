using System;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.Views.Settings;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels.Settings
{
    [NotifyForAll]
    public class SettingsBgViewModel : Screen, IChildT<SettingsPageViewModel>
    {
        protected BackgroundImageService BackgroundImageService { get; set; }
        protected SettingsService SettingsService { get; set; }

        private readonly object _sliderGate = new object();
        private IDisposable _sliderDisposable;

        public bool IsBgEnabled { get; set; }

        public double OpacityValue { get; set; }

        public bool IsSettingsChanged { get; set; }

        public SettingsBgViewModel(
            SettingsService settingsService,
            BackgroundImageService backgroundImageService)
        {
            SettingsService = settingsService;
            BackgroundImageService = backgroundImageService;

            IsBgEnabled = BackgroundImageService.IsEnabled;
            OpacityValue = backgroundImageService.Opacity;
        }

        protected override void OnDeactivate(bool close)
        {
            if (IsSettingsChanged)
            {
                SettingsService.SaveBackgroundImageSettings();
                IsSettingsChanged = false;
            }

            base.OnDeactivate(close);
        }

        public void OnSwitchChanged()
        {
            IsSettingsChanged = true;
            BackgroundImageService.ChangeEnabled(this.GetParent().GetView(), IsBgEnabled);
        }

        public void SliderValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            IsSettingsChanged = true;
            BackgroundImageService.ChangeOpacity(this.GetParent().GetView(), e.NewValue);
        }

        public void SliderStarted(object view)
        {
            ChangeHitTest(true);
        }

        /// <summary>
        /// Trigger when slider maniplation completed.
        /// </summary>
        /// <remarks>
        /// Unlocks pivot after 0.4 seconds delay.
        /// </remarks>
        /// <param name="view"></param>
        public void SliderCompleted(object view)
        {
            lock (_sliderGate)
            {
                if (_sliderDisposable != null)
                {
                    _sliderDisposable.Dispose();
                }
                var obs = Observable.Timer(TimeSpan.FromSeconds(0.4))
                    .Synchronize(_sliderGate)
                    .ObserveOnDispatcher();
                _sliderDisposable = obs.Subscribe(tick => ChangeHitTest(false));
            }
        }

        private void ChangeHitTest(bool value)
        {
            var uiView = this.GetParent().GetView() as SettingsPage;
            if (uiView != null)
            {
                uiView.SettingsPivot.IsLocked = value;
            }
        }

        public void ChangeBackground()
        {
            BackgroundImageService.SelectFromPhotoChooser();
        }
    }
}
