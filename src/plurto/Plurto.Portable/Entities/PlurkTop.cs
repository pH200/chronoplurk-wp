using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Plurto.Entities
{
    public sealed class PlurkTopCollection
    {
        public string EnglishName { get; set; }

        public IList<string> Languages { get; set; }

        public string Name { get; set; }
    }

    public sealed class PlurkTopTopic
    {
        public int Filter { get; set; }

        public string Name { get; set; }

        public string EnglishName { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class PlurkTopResult : ITimeline
    {
        [JsonProperty("plurk_users")]
        public IDictionary<int, User> Users { get; set; }

        [JsonProperty("plurks")]
        public IList<Plurk> Plurks { get; set; }

        [JsonProperty("offset")]
        public double Offset { get; set; }

        public int GetUserIdFromPlurk(Plurk plurk)
        {
            return plurk.OwnerId;
        }
    }
}
