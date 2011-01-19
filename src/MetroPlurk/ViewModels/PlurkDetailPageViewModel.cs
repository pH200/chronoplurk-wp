using System;
using System.Linq;
using System.Windows.Media;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using Plurto.Core;

namespace MetroPlurk.ViewModels
{
    public class PlurkDetailPageViewModel : LoginAvailablePage
    {
        private readonly IPlurkService _plurkService;

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public Qualifier QualifierEnum { get; set; }

        public string Qualifier { get; set; }

        public Color QualifierColor
        {
            get { return QualifierConverter.ConvertQualifierColor(QualifierEnum); }
        }

        public DateTime PostDate { get; set; }

        public string TimeView { get { return PostDate.ToShortDateString(); } }

        public string ContentRaw { get; set; }

        public string Content { get; set; }

        public string AvatarView { get; set; }

        public CommentMode NoComments { get; set; }

        private bool _isFavorite;

        public bool IsFavorite
        {
            get { return _isFavorite; }
            set
            {
                if (_isFavorite == value) return;
                _isFavorite = value;
                NotifyOfPropertyChange(() => IsFavorite);
                NotifyOfPropertyChange(() => LikeText);
            }
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

        private UnreadStatus _isUnread;

        public UnreadStatus IsUnread
        {
            get { return _isUnread; }
            set
            {
                if (_isUnread == value) return;
                _isUnread = value;
                NotifyOfPropertyChange(() => IsUnread);
                NotifyOfPropertyChange(() => MuteText);
            }
        }

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
