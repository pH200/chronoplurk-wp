using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CometData
    {
        [JsonProperty("new_offset")]
        public int NewOffset { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
