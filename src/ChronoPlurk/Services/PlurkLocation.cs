using System;
using System.Linq;
using ChronoPlurk.ViewModels;
using Plurto.Core;

namespace ChronoPlurk.Services
{
    public class PlurkLocation
    {
        private const string PageUri =
            "/Views/PlurkDetailPage.xaml?" +
            "PlurkId={0}&UserId={1}&Username={2}" +
            "&QualifierEnumInt={3}&Qualifier={4}" +
            "&PostDateTicks={5}&" +
            "&AvatarView={6}&NoCommentsInt={7}&IsFavorite={8}"+
            "&ResponseCount={9}&IsUnreadInt={10}" + 
            "&PlurkTypeInt={11}" +
            "&IsReplurkable={12}&IsReplurked={13}";

        public PlurkLocation(PlurkItemViewModel item)
        {
            Parsed = HttpFormat(PageUri, item.PlurkId, item.UserId, item.Username, (int)item.QualifierEnum, item.Qualifier,
                                item.PostDate.ToUniversalTime().Ticks, item.AvatarView, (int)item.NoComments, item.IsFavorite,
                                item.ResponseCount, (int)item.IsUnread, (int)item.PlurkType,
                                item.IsReplurkable, item.IsReplurked);
        }

        private static string HttpFormat(string format, params object[] args)
        {
            var array = args.Select(arg =>
            {
                var value = ObjectToString(arg);
                return (object) HttpTools.EscapeDataStringOmitNull(value);
            }).ToArray();
            return string.Format(format, array);
        }

        private static string ObjectToString(object value)
        {
            return value == null ? null : value.ToString();
        }

        public string Parsed { get; private set; }

        public Uri ToUri()
        {
            return new Uri(Parsed, UriKind.Relative);
        }
    }
}
