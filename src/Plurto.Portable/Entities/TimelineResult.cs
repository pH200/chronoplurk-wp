using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class TimelineResult : ITimeline
    {
        [JsonProperty("plurk_users")]
        public IDictionary<int, User> Users { get; set; }

        [JsonProperty("plurks")]
        public IList<Plurk> Plurks { get; set; }

        public int GetUserIdFromPlurk(Plurk plurk)
        {
            return plurk.OwnerId;
        }
    }
}
