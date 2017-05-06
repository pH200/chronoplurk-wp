using System;
using System.Text.RegularExpressions;

namespace Plurto.Helpers
{
    public static class UrlHelper
    {
        private const string PlurkPattern = @"(http|https)://(www\.)?plurk\.com/(m/)?p/(?<base36>[0-9a-zA-Z]+)/?$";

        private const string UserPattern = @"(http|https)://(www\.)?plurk\.com/(m/u/)?(?<username>[0-9a-zA-Z_]+)/?$";

        public static bool IsPlurkUrl(string url)
        {
            return Regex.IsMatch(url, PlurkPattern);
        }

        public static bool TryDecodePlurk(Uri uri, out long id)
        {
            return TryDecodePlurk(uri.AbsoluteUri, out id);
        }

        public static bool TryDecodePlurk(string url, out long id)
        {
            var match = Regex.Match(url, PlurkPattern);
            if (match.Success)
            {
                var idGroup = match.Groups["base36"];
                if (idGroup.Success)
                {
                    id = Base36.Decode(idGroup.Value);
                    return true;
                }
            }
            id = 0;
            return false;
        }

        public static bool IsUserUrl(string url)
        {
            return Regex.IsMatch(url, UserPattern);
        }

        public static bool TryDecodeUser(Uri uri, out string id)
        {
            return TryDecodeUser(uri.AbsoluteUri, out id);
        }

        public static bool TryDecodeUser(string url, out string id)
        {
            var match = Regex.Match(url, UserPattern);
            if (match.Success)
            {
                var user = match.Groups["username"];
                if (user.Success)
                {
                    id = user.Value;
                    return true;
                }
            }
            id = null;
            return false;
        }
    }
}
