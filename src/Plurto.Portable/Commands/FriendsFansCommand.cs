using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plurto.Core;
using Plurto.Entities;

namespace Plurto.Commands
{
    public static class FriendsFansCommand
    {
        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<IList<User>> GetFriendsByOffset(long userId, int? offset = null, int limit = 10)
        {
            var command = new CommandBase<IList<User>>(HttpVerb.Get, "/FriendsFans/getFriendsByOffset");
            command.AddParameter("user_id", userId);
            if (offset.HasValue)
            {
                command.AddParameter("offset", offset.Value);
            }
            command.AddParameter("limit", limit);
            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<IList<User>> GetFansByOffset(long userId, int? offset = null, int limit = 10)
        {
            var command = new CommandBase<IList<User>>(HttpVerb.Get, "/FriendsFans/getFansByOffset");
            command.AddParameter("user_id", userId);
            if (offset.HasValue)
            {
                command.AddParameter("offset", offset.Value);
            }
            command.AddParameter("limit", limit);
            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<FriendsFansCompletion> GetCompletion()
        {
            var command = new CommandBase<FriendsFansCompletion>(HttpVerb.Get, "/FriendsFans/getCompletion");
            command.Deserializer = CompletionDeserializer;

            return command;
        }

        private static FriendsFansCompletion CompletionDeserializer(ResponseData reponseData)
        {
            var obj = JObject.Parse(reponseData.Body);
            var completion = new FriendsFansCompletion()
            {
                Lookup = new Dictionary<int, CompletionUser>(obj.Count)
            };
            foreach (var property in obj.Properties())
            {
                int id;
                if (int.TryParse(property.Name, out id))
                {
                    var user = new CompletionUser() { Id = id };
                    foreach (var childProperty in property.Value.Children<JProperty>())
                    {
                        switch (childProperty.Name)
                        {
                            case "nick_name":
                                user.NickName = (string)childProperty.Value;
                                break;
                            case "display_name":
                                user.DisplayName = (string)childProperty.Value;
                                break;
                            case "full_name":
                                user.FullName = (string)childProperty.Value;
                                break;
                        }
                    }
                    completion.Lookup.Add(id, user);
                }
            }
            return completion;
        }
    }
}
