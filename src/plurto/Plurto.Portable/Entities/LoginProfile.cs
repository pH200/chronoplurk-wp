using System.Net;

namespace Plurto.Entities
{
    public sealed class LoginProfile
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public CookieCollection Cookies { get; set; }
        
        public Profile Profile { get; set; }

        public User UserInfo
        {
            get { return Profile.UserInfo; }
            set { Profile.UserInfo = value; }
        }
    }
}
