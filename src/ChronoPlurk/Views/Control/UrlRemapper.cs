using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChronoPlurk.Views.PlurkControls
{
    public static class UrlRemapper
    {
        private static PredicateMapper[] _remappers = new PredicateMapper[]
        {
            new PredicateMapper(
                predicate: u =>
                {
                    return u.StartsWith("http://www.youtube.com/");
                }, mapper: u =>
                {
                    return new Uri(u.Replace("http://www.youtube.com/", "http://m.youtube.com/#/"), UriKind.Absolute);
                }),
            new PredicateMapper(
                predicate: u =>
                {
                    return u.StartsWith("http://youtu.be/");
                }, mapper: u =>
                {
                    const string mobileUrl = "http://m.youtube.com/#/watch?v=";
                    const string pattern = @"http://youtu.be/([^\?\&]+)";
                    var match = Regex.Match(u, pattern);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        var newUrl = mobileUrl + match.Groups[1];
                        return new Uri(newUrl, UriKind.Absolute);
                    }
                    else
                    {
                        return new Uri(u, UriKind.Absolute);
                    }
                }),
        };

        public static Uri RemapUrl(string url)
        {
            var remapper = _remappers.FirstOrDefault(r => r.Predicate(url));
            if (remapper != null)
            {
                return remapper.Mapper(url);
            }
            else
            {
                return new Uri(url, UriKind.Absolute);
            }
        }

        private class PredicateMapper
        {
            public Func<string, bool> Predicate { get; private set; }
            public Func<string, Uri> Mapper { get; private set; }

            public PredicateMapper(Func<string, bool> predicate, Func<string, Uri> mapper)
            {
                Predicate = predicate;
                Mapper = mapper;
            }
        }
    }
}
