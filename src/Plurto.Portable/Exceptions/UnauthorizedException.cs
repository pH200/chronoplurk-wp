using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plurto.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
        {
        }

        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
