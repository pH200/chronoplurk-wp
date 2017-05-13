using Plurto.Core;
using Plurto.Entities;

namespace Plurto.Commands
{
    public static class EmoticonsCommand
    {
        public static CommandBase<Emoticons> Get()
        {
            var command = new CommandBase<Emoticons>(HttpVerb.Get, "/Emoticons/get");

            command.SetDefaultJsonDeserializer();

            return command;
        }
    }
}
