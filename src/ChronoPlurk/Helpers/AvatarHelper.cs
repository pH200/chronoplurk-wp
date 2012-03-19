using Plurto.Entities;

namespace ChronoPlurk.Helpers
{
    public static class AvatarHelper
    {
        public static string MapAvatar(string avatar)
        {
            if (avatar.Contains("www.plurk.com/static/default_"))
            {
                return "Resources/Avatar/default_big.jpg";
            }
            else
            {
                return avatar;
            }
        }

        public static string MapAvatar(User user)
        {
            return MapAvatar(user.AvatarBig);
        }
    }
}
