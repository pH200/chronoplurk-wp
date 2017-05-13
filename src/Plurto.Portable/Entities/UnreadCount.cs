using Newtonsoft.Json;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UnreadCount
    {
        [JsonProperty("all")]
        public int All { get; set; }

        [JsonProperty("my")]
        public int My { get; set; }

        [JsonProperty("private")]
        public int Private { get; set; }

        [JsonProperty("responded")]
        public int Responded { get; set; }
    }
}
