using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Commands;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class PlurkDetailHeaderViewModel : Screen
    {
        private readonly IPlurkContentStorageService _plurkContentStorageService;
        private readonly INavigationService _navigationService;
        private readonly IPlurkService _plurkService;
        private readonly IProgressService _progressService;

        private IDisposable _getDisposable;

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
            IPlurkService plurkService,
            IProgressService progressService)
        {
            _plurkContentStorageService = plurkContentStorageService;
            _navigationService = navigationService;
            _plurkService = plurkService;
            _progressService = progressService;
        }

        protected override void OnActivate()
        {
            var content = _plurkContentStorageService.GetValueOrDefault(PlurkId);
            if (content != null)
            {
                ContentHtml = content;
            }
            _plurkContentStorageService.Remove(PlurkId);

            if (Username == null) // Uninitialized
            {
                Initialize(PlurkId);
            }

            base.OnActivate();
        }

        public void OnUserTap()
        {
            if (_plurkService.IsLoaded)
            {
                _navigationService.GotoProfilePage(UserId, Username, AvatarView);
            }
        }

        private void Initialize(long id)
        {
            if (_getDisposable != null)
            {
                _getDisposable.Dispose();
            }

            _progressService.Show(AppResources.msgLoading);

            var command = TimelineCommand.GetPlurk(id)
                .Client(_plurkService.Client)
                .ToObservable()
                .PlurkException();
            _getDisposable = command.Subscribe(up =>
            {
                UserId = up.User.Id;
                Username = up.User.DisplayNameOrNickName;
                ContentHtml = up.Plurk.Content;
                QualifierEnumInt = (int)up.Plurk.Qualifier;
                Qualifier = up.Plurk.QualifierTranslatedOrDefault;
                PostDateTicks = up.Plurk.PostDate.ToUniversalTime().Ticks;
                AvatarView = AvatarHelper.MapAvatar(up.User);
                NoCommentsInt = (int)up.Plurk.NoComments;
                IsFavorite = up.Plurk.Favorite;
                ResponseCount = up.Plurk.ResponseCount;
                IsUnreadInt = (int)up.Plurk.IsUnread;
                PlurkTypeInt = (int)up.Plurk.PlurkType;
            }, () => Execute.OnUIThread(_progressService.Hide));
        }
    }
}
