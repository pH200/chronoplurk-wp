using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SearchResult : ITimeline
    {
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("last_offset")]
        public int? LastOffset { get; set; }

        [JsonProperty("users")]
        public IDictionary<int, User> Users { get; set; }

        [JsonProperty("plurks")]
        public IList<Plurk> Plurks { get; set; }

        public int GetUserIdFromPlurk(Plurk plurk)
        {
            return plurk.OwnerId;
        }
    }
}
