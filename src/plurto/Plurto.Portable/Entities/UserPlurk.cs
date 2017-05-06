using System;
using Newtonsoft.Json;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UserPlurk
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("plurk")]
        public Plurk Plurk { get; set; }

        public override string ToString()
        {
            if (User == null || Plurk == null)
            {
                return base.ToString();
            }
            return String.Format("User: {0}, Plurk: {1}", User.NickName, Plurk.ContentRaw);
        }
    }
}
