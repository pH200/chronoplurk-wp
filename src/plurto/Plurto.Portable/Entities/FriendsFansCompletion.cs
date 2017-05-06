using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Plurto.Entities
{
    public sealed class FriendsFansCompletion
    {
        public IDictionary<int, CompletionUser> Lookup { get; set; }

        public IEnumerable<CompletionUser> Search(string name)
        {
            if (Lookup == null || Lookup.Count == 0)
            {
                return Enumerable.Empty<CompletionUser>();
            }
            else
            {
                var result = from user in Lookup.Values
                             where (user.NickName != null && user.NickName.Contains(name))
                                   || (user.DisplayName != null && user.DisplayName.Contains(name))
                             select user;
                return result;
            }
        }
    }
    
    public struct CompletionUser
    {
        public int Id { get; set; }

        [JsonProperty("nick_name")]
        public string NickName { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        public override string ToString()
        {
            return String.Format("Id: {0}, NickName: {1}, DisplayName: {2}, FullName: {3}",
                                 Id,
                                 NickName,
                                 DisplayName,
                                 FullName);
        }
    }
}
