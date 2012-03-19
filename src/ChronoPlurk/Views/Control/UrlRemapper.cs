using System;
using System.Linq;
using System.Text.RegularExpressions;
using Plurto.Helpers;

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
                    return UriFactory(u.Replace("http://www.youtube.com/", "http://m.youtube.com/#/"), UriKind.Absolute);
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
                        return UriFactory(newUrl, UriKind.Absolute);
                    }
                    else
                    {
                        return UriFactory(u, UriKind.Absolute);
                    }
                }),
            new PredicateMapper(
                predicate: UrlHelper.IsPlurkUrl,
                mapper: u =>
                {
                    const string baseUrl = "/Views/PlurkDetailPage.xaml?PlurkId=";
                    long id;
                    if (UrlHelper.TryDecodePlurk(u, out id))
                    {
                        var newUrl = baseUrl + id;
                        return UriFactory(newUrl, UriKind.Relative);
                    }
                    else
                    {
                        return UriFactory(u, UriKind.Absolute);
                    }
                }),
            new PredicateMapper(
                predicate: UrlHelper.IsUserUrl,
                mapper: u =>
                {
                    const string baseUrl = "/Views/Profile/PlurkProfilePage.xaml?Username=";
                    string id;
                    if (UrlHelper.TryDecodeUser(u, out id))
                    {
                        var newUrl = baseUrl + id;
                        return UriFactory(newUrl, UriKind.Relative);
                    }
                    else
                    {
                        return UriFactory(u, UriKind.Absolute);
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
                return UriFactory(url, UriKind.Absolute);
            }
        }

        private static Uri UriFactory(string url, UriKind uriKind)
        {
            if (uriKind == UriKind.Relative)
            {
                return new Uri(url, uriKind);
            }
            else
            {
                if (Uri.IsWellFormedUriString(url, uriKind))
                {
                    return new Uri(url, uriKind);
                }
                else
                {
                    return new Uri("http://m.bing.com/search?q=" + Uri.EscapeDataString(url), UriKind.Absolute);
                }
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
