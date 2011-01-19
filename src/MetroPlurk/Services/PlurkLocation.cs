using System;
using MetroPlurk.ViewModels;

namespace MetroPlurk.Services
{
    public class PlurkLocation
    {
        private const string PageUri =
            "/Views/PlurkDetailPage.xaml?Id={0}&UserId={1}&Username={2}&QualifierEnum={3}&Qualifier={4}&PostDate={5}&Content={6}&AvatarView={7}NoComments={8}&IsFavorite={9}&ResponseCount={10}&IsUnread={11}";

        public PlurkLocation(PlurkItemViewModel item)
        {
            Parsed = String.Format(PageUri, item.Id, item.UserId, item.Username, item.QualifierEnum, item.Qualifier,
                                   item.PostDate, item.Content, item.AvatarView, item.NoComments, item.IsFavorite,
                                   item.ResponseCount, item.IsUnread);
        }

        public string Parsed { get; private set; }

        public Uri ToUri()
        {
            return new Uri(Parsed, UriKind.Relative);
        }
    }
}
