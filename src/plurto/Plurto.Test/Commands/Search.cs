using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plurto.Commands;
using Plurto.Entities;
using Xunit;

namespace Plurto.Test.Commands
{
    public class Search
    {
        [Fact]
        public void StringToInt()
        {
            var result = JsonConvert.DeserializeObject<SearchResult>(File.ReadAllText("searchresult1.txt"));
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Search1()
        {
            var searchcommand = SearchCommand.PlurkSearch("Youtube ").TestClient();
            (await searchcommand.GetResponseAsync()).WriteDump();
            foreach (var plurk in (await searchcommand.LoadAsync()).ToUserPlurks())
            {
                Trace.WriteLine(plurk);
            }
        }

        [Fact]
        public async Task SearchJapanese()
        {
            var searchcommand = SearchCommand.PlurkSearch("後悔なんて、あるわけない").TestClient();
            foreach (var plurk in (await searchcommand.LoadAsync()).ToUserPlurks())
            {
                Trace.WriteLine(plurk);
            }
        }

        [Fact]
        public async Task SearchUser()
        {
            var searchcommand = SearchCommand.UserSearch("jack").TestClient();
            (await searchcommand.LoadAsync()).ToString().WriteDump();
        }
    }
}
