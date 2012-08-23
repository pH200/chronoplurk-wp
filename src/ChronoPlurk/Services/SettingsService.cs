using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Helpers;
using ChronoPlurk.Models;
using ChronoPlurk.ViewModels.Core;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Services
{
    public class SettingsService
    {
        private const string AutoRotateModeKey = "AutoRotateMode";
        private const string FiltersOnOffPackKey = "FiltersOnOffPack";
        private const string IsInfiniteScrollKey = "IsInfiniteScroll";
        private const string BgSettingsFilename = "bg_settings.bin";

        protected INavigationService NavigationService { get; set; }

        protected AutoRotateService AutoRotateService { get; set; }

        protected BackgroundImageService BackgroundImageService { get; set; }

        public SettingsService(
            INavigationService navigationService,
            AutoRotateService autoRotateService,
            BackgroundImageService backgroundImageService)
        {
            NavigationService = navigationService;
            CtorAutoRotateService(autoRotateService);
            CtorBackgroundImageService(backgroundImageService);
        }

        public void ClearToDefault()
        {
            BackgroundImageService.ChangeBgDefault();
        }

        #region Filters

        public FiltersOnOffPack GetFiltersPack()
        {
            FiltersOnOffPack pack;
            if (IsoSettings.TryGetValue(FiltersOnOffPackKey, out pack))
            {
                return pack;
            }
            else
            {
                return FiltersOnOffPack.CreateAllTrue();
            }
        }

        public void SetFiltersPack(FiltersOnOffPack pack)
        {
            IsoSettings.AddOrChange(FiltersOnOffPackKey, pack, true);
        }

        #endregion


        #region AutoRotateService

        private void CtorAutoRotateService(AutoRotateService autoRotateService)
        {
            AutoRotateService = autoRotateService;
            AutoRotateMode mode;
            if (IsoSettings.TryGetValue(AutoRotateModeKey, out mode))
            {
                ChangeAutoRotateMode(mode, true, false);
            }
        }

        public void ChangeAutoRotateMode(AutoRotateMode mode, bool applyCurrentPage = true, bool save = true)
        {
            if (save)
            {
                IsoSettings.AddOrChange(AutoRotateModeKey, mode);
            }
            AutoRotateService.SetAutoRotateMode(mode);
            if (applyCurrentPage)
            {
                var page = NavigationService.CurrentContent as PhoneApplicationPage;
                if (page != null)
                {
                    SpecialFrameAdapter.SetPageSupportedOrientation(AutoRotateService, page);
                }
            }
        }

        public int GetCurrentAutoRotateIndex()
        {
            return ((int) AutoRotateService.AutoRotateMode);
        }

        #endregion


        #region BackgroundImageService

        private void CtorBackgroundImageService(BackgroundImageService backgroundImageService)
        {
            BackgroundImageService = backgroundImageService;
            var settings = IsoSettings.DeserializeLoad(BgSettingsFilename) as BgSettings;
            if (settings != null)
            {
                BackgroundImageService.LoadSettings(settings);
            }
        }

        public void SaveBackgroundImageSettings()
        {
            var settings = BackgroundImageService.ExportSettings();
            ThreadEx.OnThreadPool(() => IsoSettings.SerializeStore(settings, BgSettingsFilename));
        }

        #endregion

        #region InfiniteScroll

        public bool GetIsInfiniteScroll()
        {
            bool isInfiniteScroll;
            if (IsoSettings.TryGetValue(IsInfiniteScrollKey, out isInfiniteScroll))
            {
                return isInfiniteScroll;
            }
            else
            {
                return true;
            }
        }

        public void SetIsInfiniteScroll(bool value)
        {
            IsoSettings.AddOrChange(IsInfiniteScrollKey, value);
        }

        #endregion

    }
}
