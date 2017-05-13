using System.Collections.Generic;
using Newtonsoft.Json;
using Plurto.Converters;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ResponsesResult : ITimeline
    {
        [JsonProperty("responses_seen")]
        public int ResponsesSeen { get; set; }

        [JsonProperty("friends")]
        [JsonConverter(typeof(EmptyArrayOrUsersConverter))]
        public IDictionary<int, User> Users { get; set; }

        [JsonProperty("responses")]
        public IList<Plurk> Plurks { get; set; }

        public int GetUserIdFromPlurk(Plurk plurk)
        {
            return plurk.UserId;
        }
    }
}
