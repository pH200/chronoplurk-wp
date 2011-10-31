using ChronoPlurk.Core;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Services
{
    public class AutoRotateService
    {
        private const string PageOrientationKey = "PageOrientation";
        private const string ComposeOrientationKey = "ComposePageOrientation";

        public AutoRotateMode AutoRotateMode { get; private set; }

        public SupportedPageOrientation PageOrientation { get; private set; }
        
        public SupportedPageOrientation ComposePageOrientation { get; private set; }

        public AutoRotateService()
        {
            SetAutoRotateMode(AutoRotateMode);
        }

        public void SetAutoRotateMode(AutoRotateMode mode)
        {
            AutoRotateMode = mode;

            SetAutoRotateModeInternal(mode);
        }

        private void SetAutoRotateModeInternal(AutoRotateMode mode)
        {
            PageOrientation = ConvertPageOrientation(mode);
            ComposePageOrientation = ConvertComposeOrientation(mode);
        }

        private static SupportedPageOrientation ConvertPageOrientation(AutoRotateMode mode)
        {
            switch (mode)
            {
                case AutoRotateMode.Always:
                    return SupportedPageOrientation.PortraitOrLandscape;

                default:
                    return SupportedPageOrientation.Portrait;
            }
        }

        private static SupportedPageOrientation ConvertComposeOrientation(AutoRotateMode mode)
        {
            switch (mode)
            {
                case AutoRotateMode.Always:
                case AutoRotateMode.ComposeOnly:
                    return SupportedPageOrientation.PortraitOrLandscape;

                default:
                    return SupportedPageOrientation.Portrait;
            }
        }
    }
}
