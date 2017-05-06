using System.Threading.Tasks;
using Plurto.Commands;
using Xunit;

namespace Plurto.Test.Commands
{
    public class Profile
    {
        [Fact]
        public async Task GetOwnProfile()
        {
            var getcmd = ProfileCommand.GetOwnProfile().TestClient();
            var profile = await getcmd.LoadAsync();
            profile.WriteDump();
        }

        [Fact]
        public async Task GetPublicProfile()
        {
            const int userid = 3218392;
            var getcmd = ProfileCommand.GetPublicProfile(userid).TestClient();
            var profile = await getcmd.LoadAsync();
            profile.WriteDump();
        }

        [Fact]
        public async Task GetPublicProfileNickname()
        {
            const string nickName = "ChronoPlurk";
            var getcmd = ProfileCommand.GetPublicProfile(nickName).TestClient();
            var profile = await getcmd.LoadAsync();
            profile.WriteDump();
        }
    }
}
