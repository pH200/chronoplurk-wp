using System.Threading.Tasks;
using Plurto.Commands;
using Plurto.Core;
using Plurto.Entities;
using Xunit;

namespace Plurto.Test.Commands
{
    public class Timeline
    {
        [Fact]
        public async Task GetPublicPlurks()
        {
            const int userid = 3218392;
            var getcmd = TimelineCommand.GetPublicPlurks(userid).TestClient();
            var value = await getcmd.LoadAsync();
            value.WriteDump();
            value.ToUserPlurks();
        }

        [Fact]
        public async Task GetUnreadPlurks()
        {
            var getcmd = TimelineCommand.GetUnreadPlurks().TestClient();
            var value = await getcmd.LoadAsync();
            value.WriteDump();
        }

        [Fact]
        public async Task GetUnreadPrivatePlurks()
        {
            var getcmd = TimelineCommand.GetUnreadPlurks(filter: UnreadFilter.Private).TestClient();
            var value = await getcmd.LoadAsync();
            value.WriteDump();
        }

        [Fact]
        public async Task Replurk()
        {
            var id = 1027738133;
            var replurkCmd = TimelineCommand.Replurk(id).TestClient();
            var unreplurkCmd = TimelineCommand.Unreplurk(id).TestClient();
            (await replurkCmd.LoadAsync()).WriteDump();
            (await unreplurkCmd.LoadAsync()).WriteDump();
        }
    }
}
