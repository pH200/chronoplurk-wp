using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Plurto.Core.OAuth
{
    public class OAuthMessageHandler : DelegatingHandler
    {
        public string AuthHeader { get; private set; }

        public OAuthMessageHandler(string authHeader)
            : base()
        {
            AuthHeader = authHeader;
        }

        public OAuthMessageHandler(string authHeader, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            AuthHeader = authHeader;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", AuthHeader);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
