using System.Windows;
using ChronoPlurk.Core;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Services
{
    public class AutoRotateService
    {
        public AutoRotateMode AutoRotateMode { get; private set; }

        public void SetAutoRotateMode(AutoRotateMode mode)
        {
            AutoRotateMode = mode;

            SetAutoRotateModeInternal(mode);
        }

        private static void SetAutoRotateModeInternal(AutoRotateMode mode)
        {
            const string pageOrientationKey = "PageOrientation";
            const string composeOrientationKey = "ComposePageOrientation";
            var resourceDict = Application.Current.Resources;
            if (resourceDict.Contains(pageOrientationKey) && resourceDict.Contains(composeOrientationKey))
            {
                var pageOrientation = ConvertPageOrientation(mode);
                var composeOrientation = ConvertComposeOrientation(mode);
                resourceDict[pageOrientationKey] = pageOrientation;
                resourceDict[composeOrientationKey] = composeOrientation;
            }
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
