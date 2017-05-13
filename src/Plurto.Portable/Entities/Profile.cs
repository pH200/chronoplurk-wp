using System.Collections.Generic;
using Newtonsoft.Json;
using Plurto.Core;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Profile : ITimeline
    {
        [JsonProperty("friends_count")]
        public int FriendsCount { get; set; }

        [JsonProperty("fans_count")]
        public int FansCount { get; set; }

        [JsonProperty("unread_count")]
        public int UnreadCount { get; set; }

        [JsonProperty("alerts_count")]
        public int AlertsCount { get; set; }

        [JsonProperty("user_info")]
        public User UserInfo { get; set; }

        [JsonProperty("privacy")]
        public UserPrivacy Privacy { get; set; }

        [JsonProperty("plurks_users")]
        public IDictionary<int, User> Users { get; set; }

        [JsonProperty("plurks")]
        public IList<Plurk> Plurks { get; set; }

        public int GetUserIdFromPlurk(Plurk plurk)
        {
            return plurk.OwnerId;
        }
    }
}
