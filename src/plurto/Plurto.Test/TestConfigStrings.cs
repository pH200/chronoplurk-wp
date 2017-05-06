using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Plurto.Test
{
    public static partial class TestConfigStrings
    {
        public static string ApiKey;

        public static string Username;

        public static string Password;

        public static string OAuthConsumerKey;

        public static string OAuthConsumerSecret;

        public static string OAuthAccessToken;

        public static string OAuthAccessTokenSecret;

        static TestConfigStrings()
        {
            if (ApiKey == null)
            {
                Func<string, string> read = (name) =>
                {
                    if (File.Exists("./" + name))
                    {
                        return File.ReadAllText("./" + name);
                    }
                    else if (File.Exists("../" + name))
                    {
                        return File.ReadAllText("../" + name);
                    }
                    else if (File.Exists("../../" + name))
                    {
                        return File.ReadAllText("../../" + name);
                    }
                    else if (File.Exists("../../../" + name))
                    {
                        return File.ReadAllText("../../../" + name);
                    }
                    else if (File.Exists("../../../../" + name))
                    {
                        return File.ReadAllText("../../../../" + name);
                    }
                    else
                    {
                        return null;
                    }
                };
                var json = read("variables.omitconf.json");
                if (json == null)
                {
                    return;
                }
                var obj = JObject.Parse(json);
                ApiKey = (string)obj["ApiKey"];
                Username = (string)obj["Username"];
                Password = (string)obj["Password"];
                OAuthConsumerKey = (string)obj["OAuthConsumerKey"];
                OAuthConsumerSecret = (string)obj["OAuthConsumerSecret"];
                OAuthAccessToken = (string)obj["OAuthAccessToken"];
                OAuthAccessTokenSecret = (string)obj["OAuthAccessTokenSecret"];
            }
        }
    }
}
