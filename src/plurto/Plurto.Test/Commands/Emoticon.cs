using System.Threading.Tasks;
using Plurto.Commands;
using Xunit;

namespace Plurto.Test.Commands
{
    public class Emoticon
    {
        [Fact]
        public async Task Get()
        {
            var getcmd = EmoticonsCommand.Get().TestClient();
            var emoticon = await getcmd.LoadAsync();
            var user = await ProfileCommand.GetOwnProfile().TestClient().LoadAsync();
            var karma = await UsersCommand.GetKarmaStats().TestClient().LoadAsync();

            emoticon.GetCustomEmotions();
            emoticon.GetCustomBracktedEmoticons().WriteDump();
            emoticon.GetRecuitedEmoticons(user.FriendsCount).WriteDump();
            emoticon.GetKarmaEmoticons(karma.CurrentKarma).WriteDump();
        }
    }
}
