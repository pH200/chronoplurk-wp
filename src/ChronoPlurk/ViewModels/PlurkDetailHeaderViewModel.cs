using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class PlurkDetailHeaderViewModel : Screen
    {
        private readonly IPlurkContentStorageService _plurkContentStorageService;
        private readonly INavigationService _navigationService;
        private readonly IPlurkService _plurkService;

        #region Public Properties

        public long PlurkId { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        [NotifyProperty(AlsoNotifyFor = new[] { "QualifierColor" })]
        public int QualifierEnumInt { get; set; }

        public string Qualifier { get; set; }
        
        [DependsOn("Qualifier")]
        public Qualifier QualifierEnum { get { return (Qualifier)QualifierEnumInt; } }

        [DependsOn("Qualifier")]
        public Color QualifierColor
        {
            get { return QualifierConverter.ConvertQualifierColor(QualifierEnum); }
        }

        [DependsOn("Qualifier")]
        public Brush QualifierColorBrush
        {
            get { return new SolidColorBrush(QualifierColor); }
        }

        [NotifyProperty(AlsoNotifyFor = new[] { "TimeView" })]
        public long PostDateTicks { get; set; }

        [DependsOn("PostDateTicks")]
        public DateTime PostDate { get { return new DateTime(PostDateTicks, DateTimeKind.Utc); } }

        [DependsOn("PostDateTicks")]
        public string TimeView
        {
            get
            {
                var datetime = PostDate.ToLocalTime();
                return datetime.ToString("g");
            }
        }

        public string ContentRaw { get; set; }

        public string ContentHtml { get; set; }

        public string AvatarView { get; set; }

        public int NoCommentsInt { get; set; }

        public CommentMode NoComments { get { return (CommentMode)NoCommentsInt; } }

        public bool IsFavorite { get; set; }

        [DependsOn("IsFavorite")]
        public Visibility IsFavoriteVisibilityView
        {
            get { return IsFavorite ? Visibility.Visible : Visibility.Collapsed; }
        }

        public int ResponseCount { get; set; }

        public string ResponseCountView
        {
            get
            {
                if (ResponseCount == 1)
                {
                    return AppResources.responseSingle;
                }
                if (ResponseCount > 1)
                {
                    return ResponseCount + AppResources.responsesCount;
                }
                return string.Empty;
            }
        }

        public int IsUnreadInt { get; set; }

        [DependsOn("IsUnreadInt")]
        public UnreadStatus IsUnread { get { return (UnreadStatus)IsUnreadInt; } }

        [DependsOn("NoComments")]
        public bool CanReply
        {
            get
            {
                switch (NoComments)
                {
                    case CommentMode.None:
                        return true;
                    case CommentMode.FriendsOnly:
                        return true;
                }
                return false;
            }
        }

        [DependsOn("IsUnread")]
        public string MuteText
        {
            get { return IsUnread == UnreadStatus.Muted ? AppResources.unmute : AppResources.mute; }
        }

        [DependsOn("IsFavorite")]
        public string LikeText
        {
            get { return IsFavorite ? AppResources.unlike : AppResources.like; }
        }

        public int PlurkTypeInt { get; set; }

        [DependsOn("PlurkTypeInt")]
        public PlurkType PlurkType { get { return (PlurkType)PlurkTypeInt; } }

        [DependsOn("PlurkTypeInt")]
        public Visibility IsPrivateVisibilityView
        {
            get
            {
                var isPrivate = (PlurkType == PlurkType.Private)
                                || (PlurkType == PlurkType.PrivateResponded);
                return isPrivate ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        public PlurkDetailHeaderViewModel(
            IPlurkContentStorageService plurkContentStorageService,
            INavigationService navigationService,
            IPlurkService plurkService)
        {
            _plurkContentStorageService = plurkContentStorageService;
            _navigationService = navigationService;
            _plurkService = plurkService;
        }

        protected override void OnActivate()
        {
            var content = _plurkContentStorageService.GetValueOrDefault(PlurkId);
            if (content != null)
            {
                ContentHtml = content;
            }
            _plurkContentStorageService.Remove(PlurkId);

            base.OnActivate();
        }

        public void OnUserTap()
        {
            if (_plurkService.IsLoaded)
            {
                _navigationService.GotoProfilePage(UserId, Username, AvatarView);
            }
        }
    }
}
