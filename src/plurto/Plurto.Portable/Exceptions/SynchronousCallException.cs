using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plurto.Exceptions
{
    public sealed class SynchronousWebRequestException : Exception
    {
        private const string DefaultMessage =
            "By default, synchronous web request is prohibited." +
            " For enabling synchronous web requests," +
            " set Config.AlwaysThrowOnSynchronousWebRequest to false";

        public SynchronousWebRequestException()
            : base(DefaultMessage)
        {
        }
    }
}
