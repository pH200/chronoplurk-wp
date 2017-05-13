using System;
using Newtonsoft.Json;
using Plurto.Core;

namespace Plurto.Exceptions
{
    public sealed class PlurtoJsonSerializationException : Exception
    {
        public ResponseData Response { get; private set; }

        public PlurtoJsonSerializationException(ResponseData responseData, JsonSerializationException e)
            : base(e.Message, e)
        {
            Response = responseData;
        }
    }
}
