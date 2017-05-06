using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Plurto.Converters;
using Plurto.Core;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Plurk
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("plurk_id")]
        public long PlurkId { get; set; }

        [JsonProperty("qualifier")]
        public Qualifier Qualifier { get; set; }

        [JsonProperty("qualifier_translated")]
        public string QualifierTranslated { get; set; }

        [JsonProperty("is_unread")]
        public UnreadStatus IsUnread { get; set; }

        [JsonProperty("plurk_type")]
        public PlurkType PlurkType { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("owner_id")]
        public int OwnerId { get; set; }

        [JsonProperty("posted")]
        [JsonConverter(typeof(Rfc1123DateTimeConverter))]
        public DateTime PostDate { get; set; }

        [JsonProperty("response_count")]
        public int ResponseCount { get; set; }

        [JsonProperty("responses_seen")]
        public int ResponsesSeen { get; set; }

        [JsonProperty("limited_to")]
        [JsonConverter(typeof(LimitedToConverter))]
        public IEnumerable<int> LimitedTo { get; set; }

        [JsonProperty("favorite")]
        public bool Favorite { get; set; }

        [JsonProperty("favorite_count")]
        public int FavoriteCount { get; set; }

        [JsonProperty("favorers")]
        public IEnumerable<int> Favorers { get; set; }

        [JsonProperty("replurkable")]
        public bool Replurkable { get; set; }

        /// <summary>
        /// True if plurk has been replurked by current user.
        /// Could be null on polling/getPlurks or profile/getPublicProfile
        /// </summary>
        [JsonProperty("replurked")]
        public bool? Replurked { get; set; }

        [JsonProperty("replurker_id")]
        [JsonConverter(typeof(ReplurkerIdConverter))]
        public int? ReplurkerId { get; set; }

        [JsonProperty("replurkers_count")]
        public int ReplurkersCount { get; set; }

        [JsonProperty("replurkers")]
        public IEnumerable<int> Replurkers { get; set; }

        [JsonProperty("no_comments")]
        public CommentMode NoComments { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        [JsonProperty("content_raw")]
        public string ContentRaw { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        public string QualifierTranslatedOrDefault
        {
            get { return !String.IsNullOrEmpty(QualifierTranslated) ? QualifierTranslated : Qualifier.ToKey(); }
        }

        public TimeSpan PostTimeFromNow
        {
            get { return DateTime.Now - PostDate; }
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}] {2}", PlurkId, Id, ContentRaw);
        }
    }
}
