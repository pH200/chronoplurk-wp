using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Microsoft.Phone.Controls;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels.Settings
{
    [NotifyForAll]
    public class SettingsPageViewModel : LoginAvailablePage
    {
        protected SettingsService SettingsService { get; set; }

        protected IPlurkService PlurkService { get; set; }

        protected INavigationService NavigationService { get; set; }

        public string LoginAccount { get; set; }

        public IList<string> AutoRotates { get; set; }

        public int AutoRotatesSelectedIndex { get; set; }

        public string VersionText { get; set; }

        public SettingsPageViewModel(
            SettingsService settingsService,
            IPlurkService plurkService,
            INavigationService navigationService,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            SettingsService = settingsService;
            PlurkService = plurkService;
            NavigationService = navigationService;

            AutoRotates = new ObservableCollection<string>()
            {
                AppResources.autoRotateComposeOnly,
                AppResources.autoRotateAlways,
                AppResources.autoRotateNever,
            };
            AutoRotatesSelectedIndex = SettingsService.GetCurrentAutoRotateIndex();

            VersionText = "1.1";
        }

        protected override void OnActivate()
        {
            LoginAccount = PlurkService.Username;
            base.OnActivate();
        }

        public void LogoutButton()
        {
            PlurkService.ClearUserData();
            NavigationService.GotoMainPage(true);
        }

        public void OssCreditsButton()
        {
            NavigationService.Navigate(new Uri("/Views/Settings/SettingsOssCreditsPage.xaml", UriKind.Relative));
        }

        public void OnAutoRotateSelectionChanged(FrameworkElement fe)
        {
            var listPicker = fe as ListPicker;
            if (listPicker != null)
            {
                var mode = GetAutoRotateModeFromIndex(listPicker.SelectedIndex);
                SettingsService.ChangeAutoRotateMode(mode, true);
            }
        }

        private static AutoRotateMode GetAutoRotateModeFromIndex(int index)
        {
            if (index >= 0 && index < 3)
            {
                return ((AutoRotateMode)index);
            }
            else
            {
                return default(AutoRotateMode);
            }
        }
    }
}
