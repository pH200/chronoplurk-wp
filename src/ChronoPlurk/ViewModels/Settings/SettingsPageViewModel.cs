using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Services;
using Microsoft.Phone.Controls;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels.Settings
{
    [NotifyForAll]
    public class SettingsPageViewModel : LoginAvailablePage
    {
        protected IPlurkService PlurkService { get; set; }

        protected INavigationService NavigationService { get; set; }

        public string LoginAccount { get; set; }

        public IList<string> AutoRotates { get; set; }

        public string VersionText { get; set; }

        public SettingsPageViewModel(
            IPlurkService plurkService,
            INavigationService navigationService,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            PlurkService = plurkService;
            NavigationService = navigationService;

            AutoRotates = new ObservableCollection<string>()
            {
                "Compose only",
                "Always",
                "Never",
            };
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
            ShowLoginPopup(true);
        }

        public void OssCreditsButton()
        {
            NavigationService.Navigate(new Uri("/Views/Settings/SettingsOssCreditsPage.xaml", UriKind.Relative));
        }

        public void OnAutoRotateSelectionChanged(SelectionChangedEventArgs e)
        {
            var listPicker = e.OriginalSource as ListPicker;
            if (listPicker != null)
            {
                
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
