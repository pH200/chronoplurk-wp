using System.Diagnostics;
using System.Threading.Tasks;
using Plurto.Commands;
using Xunit;

namespace Plurto.Test.Commands
{
    public class Users
    {
        [Fact]
        public async Task CurrUser()
        {
            // currUser is only supported by Plurk OAuth API.
            var getcmd = UsersCommand.CurrentUser().Client(TestConfig.OAuthClient);
            (await getcmd.LoadAsync()).WriteDump();
        }

        [Fact]
        public async Task GetKarmaStats()
        {
            var karmacmd = UsersCommand.GetKarmaStats().TestClient();
            Trace.WriteLine(await karmacmd.LoadAsync());
        }
    }
}
