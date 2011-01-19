using System;
using System.Linq;
using System.Windows.Media;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Core;

namespace MetroPlurk.ViewModels
{
    [NotifyForAll]
    public class PlurkDetailPageViewModel : LoginAvailablePage
    {
        private readonly IPlurkService _plurkService;

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        [NotifyProperty(AlsoNotifyFor = new []{"QualifierColor"})]
        public int QualifierEnumInt { get; set; }
        
        public Qualifier QualifierEnum { get { return (Qualifier) QualifierEnumInt; } }

        public string Qualifier { get; set; }

        public Color QualifierColor
        {
            get { return QualifierConverter.ConvertQualifierColor(QualifierEnum); }
        }

        [NotifyProperty(AlsoNotifyFor = new []{"TimeView"})]
        public long PostDateTicks { get; set; }

        public DateTime PostDate { get { return new DateTime(PostDateTicks, DateTimeKind.Local); } }

        public string TimeView { get { return PostDate.ToString(); } }

        public string ContentRaw { get; set; }

        public string Content { get; set; }

        public string AvatarView { get; set; }

        public int NoCommentsInt { get; set; }

        public CommentMode NoComments { get { return (CommentMode) NoCommentsInt; } }

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

        public UnreadStatus IsUnread { get { return (UnreadStatus) IsUnreadInt; } }

        public bool CanReply
        {
            get
            {
                switch (NoComments)
                {
                    case CommentMode.None:
                        return true;
                    case CommentMode.FriendsOnly:
                        return (_plurkService != null &&
                                _plurkService.FriendsId != null &&
                                _plurkService.FriendsId.Contains(UserId));
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
        
        public PlurkDetailPageViewModel
            (IPlurkService plurkService,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            _plurkService = plurkService;
        }
    }
}
