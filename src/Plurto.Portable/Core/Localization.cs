using System;
using System.Globalization;

namespace Plurto.Core
{
    public static class LanguageHelper
    {
        public static PlurkLanguage? CultureInfoToPlurkLang(CultureInfo culture)
        {
            var cultureName = culture.Name;
            switch (cultureName)
            {
                case "en-US":
                case "en-GB":
                case "en-AU":
                case "en-CA":
                case "en-NZ":
                    return PlurkLanguage.en;
                case "zh-TW":
                case "zh-HK":
                    return PlurkLanguage.tr_ch;
            }
            return null;
        }
    }

    public static class QualifierLocalization
    {
        public static string ToLocalized(this Qualifier qualifier, CultureInfo culture)
        {
            var lang = LanguageHelper.CultureInfoToPlurkLang(culture);
            return qualifier.ToLocalized(lang);
        }

        public static string ToLocalized(this Qualifier qualifier, PlurkLanguage? lang)
        {
            switch (lang)
            {
                case PlurkLanguage.en:
                    return qualifier.ToKey();
                case PlurkLanguage.tr_ch:
                    return QualifierZhTw.GetLocalized(qualifier);
            }
            return qualifier.ToKey();
        }

        public static string ToLocalizedSpecific(this Qualifier qualifier, string language)
        {
            switch (language)
            {
                case "zh-TW":
                    return QualifierZhTw.GetLocalized(qualifier);
            }
            return qualifier.ToKey();
        }
    }

    public static class QualifierZhTw
    {
        public static string GetLocalized(Qualifier qualifier)
        {
            switch (qualifier)
            {
                case Qualifier.FreestyleColon: return ": (自由發揮)";
                case Qualifier.Freestyle: return ": (自由發揮)";
                case Qualifier.Loves: return "愛";
                case Qualifier.Likes: return "喜歡";
                case Qualifier.Shares: return "分享";
                case Qualifier.Gives: return "給";
                case Qualifier.Hates: return "討厭";
                case Qualifier.Wants: return "想要";
                case Qualifier.Wishes: return "期待";
                case Qualifier.Needs: return "需要";
                case Qualifier.Will: return "打算";
                case Qualifier.Hopes: return "希望";
                case Qualifier.Asks: return "問";
                case Qualifier.Has: return "已經";
                case Qualifier.Was: return "曾經";
                case Qualifier.Wonders: return "好奇";
                case Qualifier.Feels: return "覺得";
                case Qualifier.Thinks: return "想";
                case Qualifier.Says: return "說";
                case Qualifier.Is: return "正在";
            }
            return qualifier.ToKey();
        }
    }
}
