using System;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Core;

namespace MetroPlurk.ViewModels
{
    [NotifyForAll]
    public class PlurkDetailHeaderViewModel : Screen
    {
        #region Public Properties

        public int Id { get; set; }

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

        public DateTime PostDate { get { return new DateTime(PostDateTicks, DateTimeKind.Local); } }

        public string TimeView { get { return PostDate.ToString(); } }

        public string ContentRaw { get; set; }

        public string Content { get; set; }

        public string AvatarView { get; set; }

        public int NoCommentsInt { get; set; }

        public CommentMode NoComments { get { return (CommentMode)NoCommentsInt; } }

        public bool IsFavorite { get; set; }

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
            get { return IsUnread == UnreadStatus.Muted ? "unmute" : "mute"; }
        }

        [DependsOn("IsFavorite")]
        public string LikeText
        {
            get { return IsFavorite ? "unlike" : "like"; }
        }

        #endregion
    }
}
