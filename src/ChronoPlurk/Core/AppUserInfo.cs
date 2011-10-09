using System.Net;

namespace ChronoPlurk.Core
{
    public sealed class AppUserInfo
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public Cookie Cookie { get; set; }

        public int UserId { get; set; }
    }
}
