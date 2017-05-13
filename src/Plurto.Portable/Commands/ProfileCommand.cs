using Plurto.Core;
using Plurto.Entities;

namespace Plurto.Commands
{
    public static class ProfileCommand
    {
        [RequireLogin]
        [RequireAccessToken]
        public static CommandBase<Profile> GetOwnProfile()
        {
            var command = new CommandBase<Profile>(HttpVerb.Get, "/Profile/getOwnProfile");
            command.SetDefaultJsonDeserializer();

            return command;
        }

        public static CommandBase<Profile> GetPublicProfile(int userId)
        {
            var command = new CommandBase<Profile>(HttpVerb.Get, "/Profile/getPublicProfile");
            command.AddParameter("user_id", userId);
            command.SetDefaultJsonDeserializer();

            return command;
        }

        public static CommandBase<Profile> GetPublicProfile(string nickName)
        {
            var command = new CommandBase<Profile>(HttpVerb.Get, "/Profile/getPublicProfile");
            command.AddParameter("user_id", nickName);
            command.SetDefaultJsonDeserializer();

            return command;
        }
    }
}
