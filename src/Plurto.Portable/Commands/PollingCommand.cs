using System;
using Plurto.Core;
using Plurto.Entities;

namespace Plurto.Commands
{
    public static class PollingCommand
    {
        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<TimelineResult> GetPlurks(DateTime offset, int? limit = null)
        {
            var command = new CommandBase<TimelineResult>(HttpVerb.Get, "/Polling/getPlurks");
            command.AddParameter("offset", offset.ToUniversalTime().ToString("s"));
            command.AddParameter("limit", limit);

            command.SetDefaultJsonDeserializer();

            return command;
        }

        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<UnreadCount> GetUnreadCount()
        {
            var command = new CommandBase<UnreadCount>(HttpVerb.Get, "/Polling/getUnreadCount");

            command.SetDefaultJsonDeserializer();

            return command;
        }
    }
}
