using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public sealed class PlurkItemViewModel : PropertyChangedBase
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public Qualifier QualifierEnum { get; set; }

        public string Qualifier { get; set; }

        public Color QualifierColor
        {
            get { return QualifierConverter.ConvertQualifierColor(QualifierEnum); }
        }

        public TimeSpan PostTimeFromNow { get; set; }

        public DateTime PostDate { get; set; }

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

        public string ResponseCountView
        {
            get
            {
                if (ResponseCount == 1)
                {
                    return "1 response";
                }
                if (ResponseCount > 1)
                {
                    return ResponseCount + " responses";
                }
                return string.Empty;
            }
        }

        public UnreadStatus IsUnread { get; set; }

        public Visibility IsUnreadView
        {
            get { return IsUnread == UnreadStatus.Unread ? Visibility.Visible : Visibility.Collapsed; }
        }

        #region Context menu related properties
        public bool ContextMenuEnabled { get; set; }

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
            get { return IsUnread == UnreadStatus.Muted ? "unmute" : "mute"; }
        }

        public string LikeText
        {
            get { return IsFavorite ? "unlike" : "like"; }
        }
        #endregion
        
        public static string ConvertTimeSpan(TimeSpan timeSpan, DateTime? postDate = null)
        {
            if (timeSpan.TotalSeconds <= 1)
            {
                return "moments ago";
            }
            if (timeSpan.TotalSeconds < 120)
            {
                return String.Format("{0} secs ago", (int)timeSpan.TotalSeconds);
            }
            if (timeSpan.TotalHours < 2)
            {
                return String.Format("{0} mins ago", (int)timeSpan.TotalMinutes);
            }
            if (timeSpan.TotalDays < 2)
            {
                return String.Format("{0} hours ago", (int)timeSpan.TotalHours);
            }
            if (postDate == null)
            {
                postDate = DateTime.Now - timeSpan;
            }
            return postDate.Value.ToString("MMM d");
        }
    }
}
