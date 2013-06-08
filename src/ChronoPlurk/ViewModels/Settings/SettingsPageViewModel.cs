using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels.Core;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels.Settings
{
    [ImplementPropertyChanged]
    public class SettingsPageViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public SettingsBgViewModel SettingsBgViewModel { get; set; }

        protected SettingsService SettingsService { get; set; }

        protected IPlurkService PlurkService { get; set; }

        protected INavigationService NavigationService { get; set; }

        public string LoginAccount { get; set; }

        public IList<string> AutoRotates { get; set; }

        public int AutoRotatesSelectedIndex { get; set; }

        public string VersionText { get; set; }

        public bool IsInfiniteScroll { get; set; }

        #region FiltersOnOff

        public bool UnreadChk { get; set; }
        public bool MyChk { get; set; }
        public bool PrivateChk { get; set; }
        public bool RespondedChk { get; set; }
        public bool Responded { get; set; }
        public bool LikedChk { get; set; }

        #endregion

        public SettingsPageViewModel(
            SettingsBgViewModel settingsBgViewModel,
            SettingsService settingsService,
            IPlurkService plurkService,
            INavigationService navigationService)
        {
            SettingsBgViewModel = settingsBgViewModel;
            SettingsBgViewModel.Parent = this;
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
            
            var pack = SettingsService.GetFiltersPack();
            SetFiltersValue(pack);

            IsInfiniteScroll = SettingsService.GetIsInfiniteScroll();

            VersionText = DefaultConfiguration.VersionText;
        }

        protected override void OnActivate()
        {
            LoginAccount = PlurkService.Username;
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            var pack = CreateFiltersOnOffPack();
            SettingsService.SetFiltersPack(pack);

            base.OnDeactivate(close);
        }

        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;
            if (pivot != null)
            {
                if (pivot.SelectedIndex == 2)
                {
                    ActivateItem(SettingsBgViewModel);
                }
                else
                {
                    DeactivateItem(SettingsBgViewModel, false);
                }
            }
        }

        public void LogoutButton()
        {
            SettingsService.ClearToDefault();
            PlurkService.ClearUserData();
            NavigationService.GotoMainPage(true);
        }

        public void OssCreditsButton()
        {
            NavigationService.Navigate(new Uri("/Views/Settings/SettingsOssCreditsPage.xaml", UriKind.Relative));
        }

        private FiltersOnOffPack CreateFiltersOnOffPack()
        {
            var pack = new FiltersOnOffPack(true, UnreadChk, MyChk, PrivateChk, RespondedChk, LikedChk);
            return pack;
        }

        private void SetFiltersValue(FiltersOnOffPack pack)
        {
            UnreadChk = pack.Unread;
            MyChk = pack.My;
            PrivateChk = pack.Private;
            RespondedChk = pack.Responded;
            LikedChk = pack.Liked;
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

        public void OnIsInfiniteScrollChanged()
        {
            SettingsService.SetIsInfiniteScroll(IsInfiniteScroll);
        }

        public void OnPlurkLink()
        {
            NavigationService.GotoProfilePage(8397265, "ChronoPlurk", "http://avatars.plurk.com/8397265-big2.jpg");
        }

        public void OnCytisanLink()
        {
            OpenExternalLink(AppResources.msgCytisanSite, () =>
            {
                var webBrowserTask = new WebBrowserTask() { Uri = new Uri("http://cytisan.com/", UriKind.Absolute) };
                webBrowserTask.Show();
            });
        }

        public void OnGrandPrizeLink()
        {
            OpenExternalLink(AppResources.wpawardsRedirectMsg, () =>
            {
                var webBrowserTask = new WebBrowserTask()
                {
                    Uri = new Uri(AppResources.wpawardsExtLink, UriKind.Absolute)
                };
                webBrowserTask.Show();
            });
        }

        private void OpenExternalLink(string message, System.Action okAction)
        {
            var msgResult = MessageBox.Show(message,
                                            AppResources.msgExternalLink,
                                            MessageBoxButton.OKCancel);
            if (msgResult == MessageBoxResult.OK)
            {
                okAction();
            }
        }
    }
}
