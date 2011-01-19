using System;
using System.Windows.Media;

namespace MetroPlurk.Helpers
{
    public static partial class PlurkResources
    {
        public static Color PlurkColor { get; private set; }
        public static Color PlurkColorBright { get; private set; }
        public static SolidColorBrush PlurkColorBrush { get; private set; }
        public static SolidColorBrush PlurkColorBrightBrush { get; private set; }
        public static SolidColorBrush RedColorBrush { get; private set; }

        static PlurkResources()
        {
            PlurkColor = Color.FromArgb(255, 207, 104, 47);
            PlurkColorBright = Color.FromArgb(255, 226, 86, 11);
            PlurkColorBrush = new SolidColorBrush(PlurkColor);
            PlurkColorBrightBrush = new SolidColorBrush(PlurkColorBright);
            RedColorBrush = new SolidColorBrush(Colors.Red);
        }

        public static SolidColorBrush PhoneForegroundBrush { get; set; }
        public static SolidColorBrush PhoneAccentBrush { get; set; }

        public static Func<double> PhoneWidthGetter;

        public static double PhoneWidth
        {
            get
            {
                return PhoneWidthGetter != null ? PhoneWidthGetter() : 0;
            }
        }

        public static Func<double> PhoneHeightGetter;

        public static double PhoneHeight
        {
            get
            {
                return PhoneHeightGetter != null ? PhoneHeightGetter() : 0;
            }
        }

#if CLEAN_DEBUG
        public static string Username { get; set; }
        public static string Password { get; set; }
#endif
    }
}
