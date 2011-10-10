using System.Collections.Generic;
using System.Net;

namespace ChronoPlurk.Core
{
    public sealed class AppUserInfo
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public Cookie[] Cookies { get; set; }

        public bool IsHasCookies
        {
            get { return (Cookies != null && Cookies.Length > 0); }
        }

        public int UserId { get; set; }
    }
}
