using System.Linq;
using System.Threading.Tasks;
using Plurto.Commands;
using Plurto.Entities;
using Xunit;

namespace Plurto.Test.Commands
{
    public class Responses
    {
        [Fact]
        public async Task Get()
        {
            var getcmd = ResponsesCommand.Get(504568987).TestClient();
            var value = await getcmd.LoadAsync();
            value.Users.WriteDump();
            value.Plurks.WriteDump();
            var zip = value.ToUserPlurks();
            if (zip != null)
            {
                var list = zip.ToList();
            }
        }
    }
}
