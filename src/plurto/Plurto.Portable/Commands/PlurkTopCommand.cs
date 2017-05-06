using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Plurto.Core;
using Plurto.Entities;

namespace Plurto.Commands
{
    public class PlurkTopCommand
    {
        public static CommandBase<IList<PlurkTopCollection>> GetCollections()
        {
            var command = new CommandBase<IList<PlurkTopCollection>>(HttpVerb.Get, "/PlurkTop/getCollections");

            command.Deserializer = response =>
            {
                var token = JToken.Parse(response.Body);
                return token.Select(element => new PlurkTopCollection()
                {
                    EnglishName = (string) element[0],
                    Languages = ((string) element[1]).Split(',').Select(s => s.Trim()).ToList(),
                    Name = (string) element[2]
                }).ToList();
            };

            return command;
        }

        public static CommandBase<IList<PlurkTopTopic>> GetTopics(string lang=null)
        {
            var command = new CommandBase<IList<PlurkTopTopic>>(HttpVerb.Get, "/PlurkTop/getTopics");
            if (lang != null)
            {
                command.AddParameter("lang", lang);
            }

            command.Deserializer = response =>
            {
                var token = JToken.Parse(response.Body);
                return token.Select(element => new PlurkTopTopic()
                {
                    Filter = (int) element[0],
                    Name = (string) element[1],
                    EnglishName = (string) element[2],
                }).ToList();
            };

            return command;
        }

        public static CommandBase<PlurkTopResult> GetPlurks(
            string collectionName,
            double? offset=null,
            int? limit=30,
            PlurkTopSorting? sorting=null,
            int? topicFilter=null)
        {
            var command = new CommandBase<PlurkTopResult>(HttpVerb.Get, "/PlurkTop/getPlurks");
            command.AddParameter("collection_name", collectionName);
            if (offset != null)
            {
                command.AddParameter("offset", offset.Value);
            }
            if (limit != null)
            {
                command.AddParameter("limit", limit.Value);
            }
            if (sorting != null)
            {
                command.AddParameter("sorting", sorting.Value.ToKey());
            }
            if (topicFilter != null)
            {
                command.AddParameter("topic_filter", topicFilter);
            }

            command.SetDefaultJsonDeserializer();

            return command;
        }
    }
}
