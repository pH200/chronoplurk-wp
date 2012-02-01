using System;
using ChronoPlurk.ViewModels;

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
            "&PlurkTypeInt={11}";

        public PlurkLocation(PlurkItemViewModel item)
        {
            Parsed = String.Format(PageUri, item.PlurkId, item.UserId, item.Username, (int)item.QualifierEnum, item.Qualifier,
                                   item.PostDate.ToLocalTime().Ticks, item.AvatarView, (int)item.NoComments, item.IsFavorite,
                                   item.ResponseCount, (int)item.IsUnread, (int)item.PlurkType);
        }

        public string Parsed { get; private set; }

        public Uri ToUri()
        {
            return new Uri(Parsed, UriKind.Relative);
        }
    }
}
