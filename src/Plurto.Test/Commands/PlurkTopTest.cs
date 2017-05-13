using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Plurto.Commands;
using Plurto.Core;
using Xunit;

namespace Plurto.Test.Commands
{
    public class PlurkTopTest
    {
        [Fact]
        public async Task GetCollections()
        {
            var cmd = PlurkTopCommand.GetCollections().Client(TestConfig.Client);
            (await cmd.LoadAsync()).WriteDump();
        }

        [Fact]
        public async Task GetTopics()
        {
            var culture = CultureInfo.GetCultureInfoByIetfLanguageTag("ja-JP");
            var plurkCulture = Plurto.Helpers.Culture.GetRecommendPlurkCulture(culture, culture);
            plurkCulture.WriteDump();
            var cmd = PlurkTopCommand.GetTopics(plurkCulture.Language).Client(TestConfig.Client);
            (await cmd.LoadAsync()).WriteDump();
        }

        [Fact]
        public async Task GetPlurks()
        {
            var culture = CultureInfo.GetCultureInfoByIetfLanguageTag("zh-TW");
            var plurkCulture = Plurto.Helpers.Culture.GetRecommendPlurkCulture(culture, culture);
            plurkCulture.WriteDump();
            var topicCmd = PlurkTopCommand.GetTopics(plurkCulture.Language).Client(TestConfig.Client);
            var topic = await topicCmd.LoadAsync();
            topic.Last().WriteDump();
            var plurksCmd = PlurkTopCommand.GetPlurks(plurkCulture.CollectionName,
                                                      sorting: PlurkTopSorting.New,
                                                      topicFilter: topic.Last().Filter)
                .Client(TestConfig.Client);
            (await plurksCmd.LoadAsync()).WriteDump();
        }
    }
}
