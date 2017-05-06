using System;
using Plurto.Core;
using Plurto.Entities;

namespace Plurto.Commands
{
    public static class ResponsesCommand
    {
        public static CommandBase<ResponsesResult> Get(long plurkId, int fromResponse = 0)
        {
            var command = new CommandBase<ResponsesResult>(HttpVerb.Get, "/Responses/get");
            command.AddParameter("plurk_id", plurkId);
            command.AddParameter("from_response", fromResponse);

            command.SetDefaultJsonDeserializer();

            return command;
        }

        /// <summary>
        /// Adds a responses to plurk_id. Language is inherited from the plurk.
        /// </summary>
        /// <param name="plurkId">The plurk that the responses should be added to.</param>
        /// <param name="content">The Plurk's text.</param>
        /// <param name="qualifier">The Plurk's qualifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when content is empty.</exception>
        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<Plurk> ResponseAdd(long plurkId, string content, Qualifier qualifier)
        {
            if (String.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content cannot be empty.", "content");
            }

            var command = new CommandBase<Plurk>(HttpVerb.Get, "/Responses/responseAdd");
            command.AddParameter("plurk_id", plurkId);
            command.AddParameter("content", content);
            command.AddParameter("qualifier", qualifier.ToKey());

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<bool> ResponseDelete(long responseId, long plurkId)
        {
            var command = new CommandBase<bool>(HttpVerb.Get, "/Responses/responseDelete");
            command.AddParameter("response_id", responseId);
            command.AddParameter("plurk_id", plurkId);

            command.SetSuccessTextDeserializer();

            return command;
        }
    }
}
