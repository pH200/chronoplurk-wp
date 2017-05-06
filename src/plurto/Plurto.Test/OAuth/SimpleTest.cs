using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Plurto.Core;
using Plurto.Core.OAuth;
using Xunit;

namespace Plurto.Test.OAuth
{
    public class SimpleTest
    {
        [Fact]
        public async Task TestRequestToken()
        {
            Func<ResponseData, List<string>> splitter = response =>
                response.Body.Split('&').Select(pair => pair.Split('=').Last()).ToList();

            var client = new OAuthClient();
            var data = new OAuthCredential()
            {
                ConsumerKey = "key",
                ConsumerSecret = "secret",
                HttpVerb = HttpVerb.Get,
            };
            var responseRequestToken = await client.Tester(data, new Uri("http://term.ie/oauth/example/request_token.php", UriKind.Absolute));
            responseRequestToken.Body.WriteDump();
            //Assert.Equal(response.Body, "oauth_token=requestkey&oauth_token_secret=requestsecret");

            var keyRequestToken = splitter(responseRequestToken);
            var token = keyRequestToken[0];
            var tokenSecret = keyRequestToken[1];

            Assert.Equal(token, "requestkey");
            Assert.Equal(tokenSecret, "requestsecret");

            data.Token = token;
            data.TokenSecret = tokenSecret;
            var responseAccessToken = await client.Tester(data, new Uri("http://term.ie/oauth/example/access_token.php", UriKind.Absolute));
            responseAccessToken.Body.WriteDump();

            var keyAccessToken = splitter(responseAccessToken);
            var accessToken = keyAccessToken[0];
            var accessTokenSecret = keyAccessToken[1];

            Assert.Equal(accessToken, "accesskey");
            Assert.Equal(accessTokenSecret, "accesssecret");

            data.Token = accessToken;
            data.TokenSecret = accessTokenSecret;
            var responseEcho = await client.Tester(data, new Uri("http://term.ie/oauth/example/echo_api.php", UriKind.Absolute));
            responseEcho.Body.WriteDump();
            //Assert.Equal(responseEcho.Body, "method=foo&bar=baz");
        }

        [Fact]
        public async Task TestPlurkOAuthSimple()
        {
            var client = new OAuthClient();
            var data = new OAuthCredential()
            {
                ConsumerKey = TestConfig.OAuthConsumerKey,
                ConsumerSecret = TestConfig.OAuthConsumerSecret,
                HttpVerb = HttpVerb.Post,
            };
            var responseRequestToken = await client.Tester(data, new Uri("http://www.plurk.com/OAuth/request_token", UriKind.Absolute));
            responseRequestToken.Body.WriteDump();
        }

        [Fact]
        public void AuthHeaderSpeedTest()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var data = new OAuthCredential()
                {
                    ConsumerKey = "test",
                    ConsumerSecret = "test",
                    HttpVerb = HttpVerb.Post,
                };
                OAuthAuthorizationHeaderBuilder.Build(data);
            }
            Trace.WriteLine(stopwatch.Elapsed);
        }
    }
}
