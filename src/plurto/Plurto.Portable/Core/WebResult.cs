using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plurto.Core
{
    public static class WebResult
    {
        public static WebResult<T> Create<T>(T result)
        {
            return new WebResult<T>(null, result);
        }

        public static WebResult<T> CreateError<T>(Exception e)
        {
            return new WebResult<T>(e, default(T));
        }
    }

    public class WebResult<T>
    {
        public Exception Error { get; private set; }

        public T Result { get; private set; }

        public WebResult(Exception error, T result)
        {
            Error = error;
            Result = result;
        }
    }
}
