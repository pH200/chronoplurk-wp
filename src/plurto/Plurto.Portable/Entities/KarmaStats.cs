using System;
using Newtonsoft.Json;
using Plurto.Core;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class KarmaStats
    {
        [JsonProperty("current_karma")]
        public double CurrentKarma { get; set; }

        [JsonProperty("karma_fall_reason")]
        public KarmaFallReason KarmaFallReason { get; set; }

        [JsonProperty("karma_graph")]
        public string KarmaGraph { get; set; }

        public override string ToString()
        {
            return String.Format("Karma: {0}, FallReason: {1}, Graph: {2}", CurrentKarma, KarmaFallReason, KarmaGraph);
        }
    }
}
