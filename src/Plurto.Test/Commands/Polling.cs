using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plurto.Commands;
using Plurto.Entities;
using Xunit;

namespace Plurto.Test.Commands
{
    public class Polling
    {
        [Fact]
        public async Task GetPlurks()
        {
            var getcmd = PollingCommand.GetPlurks(DateTime.Now.Subtract(new TimeSpan(3, 0, 0, 0))).TestClient();
            var des = await getcmd.LoadAsync();
            des.Users.WriteDump();
            des.Plurks.WriteDump();
            des.ToUserPlurks().WriteDump();
        }

        [Fact]
        public async Task GetUnreadCount()
        {
            var getuncmd = PollingCommand.GetUnreadCount().TestClient();
            //Trace.WriteLine(getuncmd.GetResponseBody());
            Trace.WriteLine(await getuncmd.LoadAsync());
        }
    }
}
