using Plurto.Commands;
using Plurto.Core;
using Plurto.Core.OAuth;

namespace Plurto.Test
{
    public static class TestConfig
    {
        public static string ApiKey = TestConfigStrings.ApiKey;

        public static string Username = TestConfigStrings.Username;

        public static string Password = TestConfigStrings.Password;

        public static string OAuthConsumerKey = TestConfigStrings.OAuthConsumerKey;

        public static string OAuthConsumerSecret = TestConfigStrings.OAuthConsumerSecret;

        public static string OAuthAccessToken = TestConfigStrings.OAuthAccessToken;

        public static string OAuthAccessTokenSecret = TestConfigStrings.OAuthAccessTokenSecret;

        static TestConfig()
        {
            InitializeClient();
        }

        public static IRequestClient Client { get; set; }

        public static OAuthClient OAuthClient { get; set; }

        private static void InitializeClient()
        {
            Config.DisableProxy = false;
            OAuthClient = new OAuthClient(OAuthConsumerKey, OAuthConsumerSecret, OAuthAccessToken, OAuthAccessTokenSecret);

            Client = OAuthClient;
        }
    }

    public static class PlurtoCommandExtensions
    {
        public static PlurkCommand<T> TestClient<T>(this CommandBase<T> command)
        {
            return command.Client(TestConfig.Client);
        }
    }
}
