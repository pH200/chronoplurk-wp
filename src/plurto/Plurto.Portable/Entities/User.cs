using System;
using Newtonsoft.Json;
using Plurto.Converters;
using Plurto.Core;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("nick_name")]
        public string NickName { get; set; }

        [JsonProperty("gender")]
        public Gender Gender { get; set; }

        [JsonProperty("has_profile_image")]
        public bool HasProfileImage { get; set; }

        [JsonProperty("avatar")]
        public int? AvatarVersion { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("date_of_birth")]
        [JsonConverter(typeof(Rfc1123DateTimeConverter))]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("relationship")]
        public string Relationship { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("page_title")]
        public string PageTitle { get; set; }

        [JsonProperty("recruited")]
        public int Recruited { get; set; }

        [JsonProperty("karma")]
        public double Karma { get; set; }

        [JsonProperty("bday_privacy")]
        public BirthdayPrivacy BirthdayPrivacy { get; set; }

        [JsonProperty("default_lang")]
        public string DefaultLanguage { get; set; }

        private string EvalAvatar(string size, string format)
        {
            if (HasProfileImage && (AvatarVersion == null || AvatarVersion == 0))
            {
                return String.Format("http://avatars.plurk.com/{0}-{1}.{2}", Id, size, format);
            }
            else if (HasProfileImage && AvatarVersion != null)
            {
                return String.Format("http://avatars.plurk.com/{0}-{1}{2}.{3}", Id, size, AvatarVersion, format);
            }
            else
            {
                return String.Format("http://www.plurk.com/static/default_{0}.gif", size);
            }
        }

        public string AvatarSmall
        {
            get { return EvalAvatar("small", "gif"); }
        }

        public string AvatarMedium
        {
            get { return EvalAvatar("medium", "gif"); }
        }

        public string AvatarBig
        {
            get { return EvalAvatar("big", "jpg"); }
        }

        public string DisplayNameOrNickName
        {
            get { return !String.IsNullOrEmpty(DisplayName) ? DisplayName : NickName; }
        }

        public override string ToString()
        {
            return String.Format("Id:{0}, Name:{1}, NickName:{2}, Gender:{3}, Karma{4}",
                                 Id, DisplayName, NickName, Gender, Karma);
        }
    }
}
