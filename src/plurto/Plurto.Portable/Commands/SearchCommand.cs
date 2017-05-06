using System;
using Newtonsoft.Json.Linq;
using Plurto.Core;
using Plurto.Entities;

namespace Plurto.Commands
{
    public static class SearchCommand
    {
        public static CommandBase<SearchResult> PlurkSearch(string query, int? offset=null)
        {
            var command = new CommandBase<SearchResult>(HttpVerb.Get, "/PlurkSearch/search");
            command.AddParameter("query", query);
            command.AddParameter("offset", offset);

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [Obsolete]
        public static CommandBase<SearchResult> Find(string query, int? offset=null)
        {
            return PlurkSearch(query, offset);
        }

        public static CommandBase<JToken> UserSearch(string query, int? offset=null)
        {
            var command = new CommandBase<JToken>(HttpVerb.Get, "/UserSearch/search");
            command.AddParameter("query", query);
            command.AddParameter("offset", offset);

            command.SetJTokenDeserializer();

            return command;
        }
    }
}
