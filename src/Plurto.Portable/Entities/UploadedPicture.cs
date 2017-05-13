using Newtonsoft.Json;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UploadedPicture
    {
        [JsonProperty("full")]
        public string Full { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }
    }
}
