using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Plurto.Helpers
{
    public static class Culture
    {
        public class PlurkCulture
        {
            public string CollectionName { get; set; }

            public string Language { get; set; }

            public PlurkCulture(string name, string lang)
            {
                CollectionName = name;
                Language = lang;
            }
        }

        public static PlurkCulture GetRecommendPlurkCulture()
        {
            return GetRecommendPlurkCulture(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);
        }

        public static PlurkCulture GetRecommendPlurkCulture(CultureInfo culture, CultureInfo uiCulture)
        {
            Func<string, string, PlurkCulture> detectEnglish = (country, lang) =>
            {
                if (uiCulture.EnglishName.StartsWith("English"))
                {
                    return new PlurkCulture(country, "en");
                }
                else
                {
                    return new PlurkCulture(country, lang);
                }
            };

            if (culture.Name == "zh-TW")
            {
                return detectEnglish("Taiwan", "tr_ch");
            }
            else if (culture.Name == "zh-HK")
            {
                return detectEnglish("HongKong", "tr_hk");
            }
            else if (culture.Name == "ko-KR")
            {
                return detectEnglish("SouthKorea", "ko");
            }
            else if (culture.Name == "ja-JP")
            {
                return detectEnglish("Japan", "ja");
            }
            else
            {
                return new PlurkCulture("English", "en");
            }
        }
    }
}
