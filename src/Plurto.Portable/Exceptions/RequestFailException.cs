using System;
using System.Net;
using Plurto.Core;

namespace Plurto.Exceptions
{
    public class RequestFailException : Exception
    {
        public RequestFailException(ResponseData response)
            : this(response, "WebRequest failed.")
        {
        }

        public RequestFailException(ResponseData response, string message)
            : base(message)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            Response = response;
        }

        public RequestFailException(ResponseData response, string message, WebException innerException)
            : base(message, innerException)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            Response = response;
            WebException = innerException;
        }

        public ResponseData Response { get; private set; }

        public WebException WebException { get; private set; }
    }
}
