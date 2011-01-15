using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MetroPlurk.Helpers
{
    public static partial class PlurkResources
    {
        public static readonly Color PlurkColor = Color.FromArgb(255, 207, 104, 47);
        public static readonly Color PlurkColorBright = Color.FromArgb(255, 226, 86, 11);
        public static readonly SolidColorBrush PlurkColorBrush = new SolidColorBrush(PlurkColor);
        public static readonly SolidColorBrush PlurkColorBrightBrush = new SolidColorBrush(PlurkColorBright);
        public static readonly SolidColorBrush RedColorBrush = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush PhoneForegroundBrush;
        public static Func<double> PhoneWidthGetter;
        public static Func<double> PhoneHeightGetter;
        public static double PhoneWidth
        {
            get
            {
                return PhoneWidthGetter != null ? PhoneWidthGetter() : 0;
            }
        }
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
