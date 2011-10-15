using System;
using ChronoPlurk.ViewModels;

namespace ChronoPlurk.Services
{
    public class PlurkLocation
    {
        private const string PageUri =
            "/Views/PlurkDetailPage.xaml?" +
            "Id={0}&UserId={1}&Username={2}" +
            "&QualifierEnumInt={3}&Qualifier={4}" +
            "&PostDateTicks={5}&ContentHtml={6}" +
            "&AvatarView={7}&NoCommentsInt={8}&IsFavorite={9}"+
            "&ResponseCount={10}&IsUnreadInt={11}";

        public PlurkLocation(PlurkItemViewModel item)
        {
            Parsed = String.Format(PageUri, item.Id, item.UserId, item.Username, (int)item.QualifierEnum, item.Qualifier,
                                   item.PostDate.ToLocalTime().Ticks, item.ContentHtml, item.AvatarView, (int)item.NoComments, item.IsFavorite,
                                   item.ResponseCount, (int)item.IsUnread);
        }

        public string Parsed { get; private set; }

        public Uri ToUri()
        {
            return new Uri(Parsed, UriKind.Relative);
        }
    }
}
