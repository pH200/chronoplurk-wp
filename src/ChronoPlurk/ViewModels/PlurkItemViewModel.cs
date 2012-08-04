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
    public sealed class PlurkItemViewModel : PropertyChangedBase
    {
        #region View Properties

        public bool EnableHyperlink { get; set; }

        #endregion

        public long PlurkId { get; set; }

        public long Id { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public Qualifier QualifierEnum { get; set; }

        public string Qualifier { get; set; }

        [DependsOn("Qualifier")]
        public Color QualifierColor
        {
            get { return QualifierConverter.ConvertQualifierColor(QualifierEnum); }
        }

        [DependsOn("Qualifier")]
        public Visibility QualifierVisibility
        {
            get { return Qualifier == null ? Visibility.Collapsed : Visibility.Visible; }
        }

        public TimeSpan PostTimeFromNow { get; set; }

        public DateTime PostDate { get; set; }

        [DependsOn("PostTimeFromNow", "PostDate")]
        public string TimeView { get { return ConvertTimeSpan(PostTimeFromNow, PostDate); } }

        public string ContentRaw { get; set; }

        public string ContentHtml { get; set; }

        public string AvatarView { get; set; }

        public CommentMode NoComments { get; set; }

        public bool IsFavorite { get; set; }

        [DependsOn("IsFavorite")]
        public Visibility IsFavoriteVisibilityView
        {
            get { return IsFavorite ? Visibility.Visible : Visibility.Collapsed; }
        }

        public int ResponseCount { get; set; }

        [DependsOn("Id")]
        public Visibility ResponseCountVisibility
        {
            get { return Id == 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        [DependsOn("ResponseCount")]
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

        public UnreadStatus IsUnread { get; set; }

        [DependsOn("IsUnread")]
        public Visibility IsUnreadView
        {
            get { return IsUnread == UnreadStatus.Unread ? Visibility.Visible : Visibility.Collapsed; }
        }

        public PlurkType PlurkType { get; set; }

        [DependsOn("PlurkType")]
        public Visibility IsPrivateVisibilityView
        {
            get
            {
                var isPrivate = (PlurkType == PlurkType.Private)
                                || (PlurkType == PlurkType.PrivateResponded);
                return isPrivate ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string ReplurkerName { get; set; }

        [DependsOn("ReplurkerName")]
        public Visibility ReplurkVisibility
        {
            get { return ReplurkerName != null ? Visibility.Visible : Visibility.Collapsed; }
        }

        public string NickName { get; set; }

        #region Context menu related properties
        
        public bool ContextMenuEnabled { get; set; }

        [DependsOn("ContextMenuEnabled")]
        public Visibility ContextMenuVisibility
        {
            get { return ContextMenuEnabled ? Visibility.Visible : Visibility.Collapsed; }
        }

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

        public string MuteText
        {
            get { return IsUnread == UnreadStatus.Muted ? AppResources.unmute : AppResources.mute; }
        }

        public string LikeText
        {
            get { return IsFavorite ? AppResources.unlike : AppResources.like; }
        }

        public int ClientUserId { get; set; }

        [DependsOn("ClientUserId", "UserId")]
        public Visibility DeleteVisibility
        {
            get { return ClientUserId == UserId ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsReplurkable { get; set; }

        [DependsOn("IsReplurkable", "ClientUserId", "UserId")]
        public Visibility CanReplurkVisibility
        {
            get
            {
                return IsReplurkable &&
                       (ClientUserId != UserId)
                           ? Visibility.Visible
                           : Visibility.Collapsed;
            }
        }

        public bool IsReplurked { get; set; }

        [DependsOn("IsReplurked")]
        public Visibility IsReplurkedVisibilityView
        {
            get { return IsReplurked ? Visibility.Visible : Visibility.Collapsed; }
        }

        public string ReplurkText
        {
            get { return IsReplurked ? AppResources.unreplurk : AppResources.replurk; }
        }

        #endregion
        
        public static string ConvertTimeSpan(TimeSpan timeSpan, DateTime? postDate = null)
        {
            if (timeSpan.TotalSeconds <= 1)
            {
                return AppResources.timeMomentsAgo;
            }
            if (timeSpan.TotalSeconds < 120)
            {
                return String.Format(AppResources.timeSecsAgo, (int)timeSpan.TotalSeconds);
            }
            if (timeSpan.TotalHours < 2)
            {
                return String.Format(AppResources.timeMinsAgo, (int)timeSpan.TotalMinutes);
            }
            if (timeSpan.TotalDays < 2)
            {
                return String.Format(AppResources.timeHoursAgo, (int)timeSpan.TotalHours);
            }
            if (postDate == null)
            {
                postDate = DateTime.Now - timeSpan;
            }
            const string numbersAndSpace = " 0123456789";
            var date = postDate.Value.ToString(AppResources.timeShort);
            if (date.All(c => numbersAndSpace.Contains(c)))
            {
                return postDate.Value.ToShortDateString();
            }
            else
            {
                return date;
            }
        }
    }
}
