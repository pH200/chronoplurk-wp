using System.Threading.Tasks;
using Plurto.Core.OAuth;
using Xunit;

namespace Plurto.Test.OAuth
{
    public class ObtainToken
    {
        [Fact]
        public async Task ObtainRequestToken()
        {
            var client = new OAuthClient(TestConfig.OAuthConsumerKey, TestConfig.OAuthConsumerSecret);
            var result = await client.ObtainRequestTokenAsync();
            Assert.NotNull(client.Token);
            Assert.NotNull(client.TokenSecret);
            result.WriteDump();
        }
    }
}
