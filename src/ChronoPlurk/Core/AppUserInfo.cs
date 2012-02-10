using System.Collections.Generic;
using System.Net;

namespace ChronoPlurk.Core
{
    public sealed class AppUserInfo
    {
        public string Username { get; set; }

        public int UserId { get; set; }

        public string UserAvatar { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        public bool HasAccessToken
        {
            get { return AccessToken != null && AccessTokenSecret != null; }
        }
    }
}
