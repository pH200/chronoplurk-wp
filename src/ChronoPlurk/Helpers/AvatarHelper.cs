using Plurto.Entities;

namespace ChronoPlurk.Helpers
{
    public static class AvatarHelper
    {
        public static string MapAvatar(User user)
        {
            if (user.AvatarBig.Contains("www.plurk.com/static/default_"))
            {
                return "Resources/Avatar/default_big.jpg";
            }
            else
            {
                return user.AvatarBig;
            }
        }
    }
}
