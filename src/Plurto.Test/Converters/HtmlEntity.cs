using Xunit;

namespace Plurto.Test.Converters
{
    public class HtmlEntity
    {
        [Fact]
        public void HtmlDecode()
        {
            const string correctOutput = "<<å&amp;&;水&#x6C34";
            var decode = Plurto.Converters.HtmlEntity.HtmlDecode("<<&#229;&amp;amp;&;&#x6C34;&#x6C34");
            Assert.Equal(correctOutput, decode);
        }
    }
}
