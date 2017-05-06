using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Plurto.Core
{
    public class ResponseData
    {
        public string Body { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public bool IsSuccessStatusCode { get; private set; }
        public Uri ResponseUri { get; private set; }

        public CookieCollection Cookies { get; set; }

        public ResponseData(string body, HttpStatusCode statusCode, bool isSuccessStatusCode, Uri responseUri)
        {
            Body = body;
            StatusCode = statusCode;
            IsSuccessStatusCode = isSuccessStatusCode;
            ResponseUri = responseUri;
        }
    }
}
