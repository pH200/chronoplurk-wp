using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Helpers;
using ChronoPlurk.ViewModels.Core;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Services
{
    public class SettingsService
    {
        private const string AutoRotateModeKey = "AutoRotateMode";
        private const string FiltersOnOffPackKey = "FiltersOnOffPack";

        protected INavigationService NavigationService { get; set; }

        protected AutoRotateService AutoRotateService { get; set; }

        public SettingsService(
            INavigationService navigationService,
            AutoRotateService autoRotateService)
        {
            NavigationService = navigationService;
            CtorAutoRotateService(autoRotateService);
        }

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
                IsoSettings.AddOrChange("AutoRotateMode", mode);
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
            return ((int)AutoRotateService.AutoRotateMode);
        }
    }
}
