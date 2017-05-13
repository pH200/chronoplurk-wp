using System.Threading.Tasks;
using Plurto.Commands;
using Xunit;

namespace Plurto.Test.Commands
{
    public class FriendsFans
    {
        [Fact]
        public async Task GetCompletion()
        {
            var getcmd = FriendsFansCommand.GetCompletion().TestClient();
            var completion = await getcmd.LoadAsync();
            completion.WriteDump();
        }

        [Fact]
        public async Task GetFriendsAndFans()
        {
            // currUser is only available for Plurk OAuth API
            var currUser = UsersCommand.CurrentUser().Client(TestConfig.OAuthClient).LoadAsync();
            var friednsCmd = FriendsFansCommand.GetFriendsByOffset(currUser.Id, 2).Client(TestConfig.Client);
            var fansCmd = FriendsFansCommand.GetFansByOffset(currUser.Id, 2).Client(TestConfig.Client);
            (await friednsCmd.LoadAsync()).WriteDump();
            (await fansCmd.LoadAsync()).WriteDump();
        }
    }
}
