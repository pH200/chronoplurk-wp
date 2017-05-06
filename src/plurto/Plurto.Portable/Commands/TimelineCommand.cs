using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Plurto.Core;
using Plurto.Entities;

namespace Plurto.Commands
{
    public static class TimelineCommand
    {
        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<UserPlurk> GetPlurk(long plurkId)
        {
            var command = new CommandBase<UserPlurk>(HttpVerb.Get, "/Timeline/getPlurk");
            command.AddParameter("plurk_id", plurkId);

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<TimelineResult> GetPlurks(DateTime? offset = null, int? limit = null, PlurksFilter filter = PlurksFilter.None)
        {
            var command = new CommandBase<TimelineResult>(HttpVerb.Get, "/Timeline/getPlurks");
            if (offset.HasValue)
            {
                command.AddParameter("offset", offset.Value.ToUniversalTime().ToString("s"));
            }
            command.AddParameter("limit", limit);
            command.AddParameter("filter", filter.ToKey());

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<TimelineResult> GetUnreadPlurks(DateTime? offset = null, int? limit = null, UnreadFilter filter = UnreadFilter.All)
        {
            var command = new CommandBase<TimelineResult>(HttpVerb.Get, "/Timeline/getUnreadPlurks");
            if (offset.HasValue)
            {
                command.AddParameter("offset", offset.Value.ToUniversalTime().ToString("s"));
            }
            command.AddParameter("limit", limit);
            if (filter != UnreadFilter.All)
            {
                command.AddParameter("filter", filter.ToKey());
            }

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<TimelineResult> GetPublicPlurks(
            int userId,
            DateTime? offset = null,
            int? limit = null)
        {
            var command = new CommandBase<TimelineResult>(HttpVerb.Get, "/Timeline/getPublicPlurks");
            command.AddParameter("user_id", userId);
            if (offset.HasValue)
            {
                command.AddParameter("offset", offset.Value.ToUniversalTime().ToString("s"));
            }
            command.AddParameter("limit", limit);

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<TimelineResult> GetPublicPlurks(
            string userId,
            DateTime? offset = null,
            int? limit = null)
        {
            var command = new CommandBase<TimelineResult>(HttpVerb.Get, "/Timeline/getPublicPlurks");
            command.AddParameter("user_id", userId);
            if (offset.HasValue)
            {
                command.AddParameter("offset", offset.Value.ToUniversalTime().ToString("s"));
            }
            command.AddParameter("limit", limit);

            command.SetDefaultJsonDeserializer();

            return command;
        }

        /// <summary>
        /// Add plurk into timeline.
        /// </summary>
        /// <param name="content">The Plurk's text.</param>
        /// <param name="qualifier">The Plurk's qualifier.</param>
        /// <param name="limitedTo">Limit the plurk only to some users.</param>
        /// <param name="noComments">Restrict commenting for this plurk.</param>
        /// <param name="lang">The plurk's language</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when content is empty.</exception>
        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<Plurk> PlurkAdd
            (string content, Qualifier qualifier,
            IEnumerable<int> limitedTo = null, CommentMode noComments = CommentMode.None, PlurkLanguage? lang = null)
        {
            if (String.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content cannot be empty.", "content");
            }
            var command = new CommandBase<Plurk>(HttpVerb.Get, "/Timeline/plurkAdd");
            command.AddParameter("content", content);
            command.AddParameter("qualifier", qualifier.ToKey());
            if (limitedTo != null)
            {
                command.AddParameter("limited_to", FormatNumbersToJson(limitedTo));
            }
            if (noComments != CommentMode.None)
            {
                command.AddParameter("no_comments", (int)noComments);
            }
            if (lang.HasValue)
            {
                command.AddParameter("lang", lang.Value.ToString());
            }

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> PlurkDelete(long plurkId)
        {
            var command = new CommandBase<bool>(HttpVerb.Get, "/Timeline/plurkDelete");
            command.AddParameter("plurk_id", plurkId);

            command.SetSuccessTextDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<Plurk> PlurkEdit(long plurkId, string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            var command = new CommandBase<Plurk>(HttpVerb.Get, "/Timeline/plurkEdit");
            command.AddParameter("plurk_id", plurkId);
            command.AddParameter("content", content);

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> MutePlurks(IEnumerable<long> ids)
        {
            var json = FormatNumbersToJson(ids);
            if (json == null)
            {
                throw new ArgumentException("Must contains at least 1 element", "ids");
            }

            var command = new CommandBase<bool>(HttpVerb.Get, "/Timeline/mutePlurks");
            command.AddParameter("ids", json);

            command.SetSuccessTextDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> MutePlurks(long id)
        {
            return MutePlurks(new[] {id});
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> UnmutePlurks(IEnumerable<long> ids)
        {
            var json = FormatNumbersToJson(ids);
            if (json == null)
            {
                throw new ArgumentException("Must contains at least 1 element", "ids");
            }

            var command = new CommandBase<bool>(HttpVerb.Get, "/Timeline/unmutePlurks");
            command.AddParameter("ids", json);

            command.SetSuccessTextDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> UnmutePlurks(long id)
        {
            return UnmutePlurks(new[] { id });
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> FavoritePlurks(IEnumerable<long> ids)
        {
            var json = FormatNumbersToJson(ids);
            if (json == null)
            {
                throw new ArgumentException("Must contains at least 1 element", "ids");
            }

            var command = new CommandBase<bool>(HttpVerb.Get, "/Timeline/favoritePlurks");
            command.AddParameter("ids", json);

            command.SetSuccessTextDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> FavoritePlurks(long id)
        {
            return FavoritePlurks(new[] { id });
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> UnfavoritePlurks(IEnumerable<long> ids)
        {
            var json = FormatNumbersToJson(ids);
            if (json == null)
            {
                throw new ArgumentException("Must contains at least 1 element", "ids");
            }

            var command = new CommandBase<bool>(HttpVerb.Get, "/Timeline/unfavoritePlurks");
            command.AddParameter("ids", json);

            command.SetSuccessTextDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> UnfavoritePlurks(long id)
        {
            return UnfavoritePlurks(new[] { id });
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<ReplurkSuccess> Replurk(IEnumerable<long> ids)
        {
            var json = FormatNumbersToJson(ids);
            if (json == null)
            {
                throw new ArgumentException("Must contains at least 1 element", "ids");
            }

            var command = new CommandBase<ReplurkSuccess>(HttpVerb.Get, "/Timeline/replurk");
            command.AddParameter("ids", json);

            command.Deserializer = ReplurkDeserializer;

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<ReplurkSuccess> Replurk(long id)
        {
            return Replurk(new[] {id});
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<ReplurkSuccess> Unreplurk(IEnumerable<long> ids)
        {
            var json = FormatNumbersToJson(ids);
            if (json == null)
            {
                throw new ArgumentException("Must contains at least 1 element", "ids");
            }

            var command = new CommandBase<ReplurkSuccess>(HttpVerb.Get, "/Timeline/unreplurk");
            command.AddParameter("ids", json);

            command.Deserializer = ReplurkDeserializer;

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<ReplurkSuccess> Unreplurk(long id)
        {
            return Unreplurk(new[] {id});
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> MarkAsRead(IEnumerable<long> ids, bool notePosition = false)
        {
            var json = FormatNumbersToJson(ids);
            if (json == null)
            {
                throw new ArgumentException("Must contains at least 1 element", "ids");
            }

            var command = new CommandBase<bool>(HttpVerb.Get, "/Timeline/markAsRead");
            command.AddParameter("ids", json);
            if (notePosition)
            {
                command.AddParameter("note_position", notePosition);
            }

            command.SetSuccessTextDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> MarkAsRead(long id, bool notePosition = false)
        {
            return MarkAsRead(new[] { id }, notePosition);
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<UploadedPicture> UploadPicture(UploadFile picture)
        {
            if (picture == null)
            {
                throw new ArgumentNullException("picture");
            }

            var command = new CommandBase<UploadedPicture>(HttpVerb.Post, "/Timeline/uploadPicture");
            picture.FieldName = "image";
            command.UploadFile = picture;

            command.SetDefaultJsonDeserializer();

            return command;
        }

        private static string FormatNumbersToJson<T>(IEnumerable<T> numbers)
        {
            if (numbers == null)
            {
                return "[]";
            }
            var sb = new StringBuilder("[");
            var isCommaAppended = false;
            foreach (var number in numbers)
            {
                sb.Append(number);
                sb.Append(',');
                isCommaAppended = true;
            }
            if (isCommaAppended)
            {
                sb.Length -= 1;
            }
            sb.Append(']');

            return sb.ToString();
        }

        private static ReplurkSuccess ReplurkDeserializer(ResponseData response)
        {
            var result = new ReplurkSuccess();
            var jobj = JObject.Parse(response.Body);
            JToken jSuccess;
            if (jobj.TryGetValue("success", out jSuccess))
            {
                result.Success = (bool)jSuccess;
            }
            JToken jResults;
            if (jobj.TryGetValue("results", out jResults))
            {
                var list = new List<KeyValuePair<long, ReplurkSuccess.ReplurkResult>>();
                foreach (var replurk in jResults.Value<JObject>().Properties())
                {
                    long id;
                    if (long.TryParse(replurk.Name, out id))
                    {
                        var results = new ReplurkSuccess.ReplurkResult();
                        foreach (var child in replurk.Value.Children<JProperty>())
                        {
                            switch (child.Name)
                            {
                                case "success":
                                    results.Success = (bool) child.Value;
                                    break;
                                case "error":
                                    results.Error = (string) child.Value;
                                    break;
                            }
                        }
                        list.Add(new KeyValuePair<long, ReplurkSuccess.ReplurkResult>(id, results));
                    }
                }
                result.Results = list;
            }
            return result;
        }
    }
}
