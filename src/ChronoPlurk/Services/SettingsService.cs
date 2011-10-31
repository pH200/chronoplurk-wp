using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Helpers;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Services
{
    public class SettingsService
    {
        private const string AutoRotateModeKey = "AutoRotateMode";

        protected INavigationService NavigationService { get; set; }

        protected AutoRotateService AutoRotateService { get; set; }

        public SettingsService(
            INavigationService navigationService,
            AutoRotateService autoRotateService)
        {
            NavigationService = navigationService;
            CtorAutoRotateService(autoRotateService);
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
