using Plurto.Core.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Plurto.Core
{
    public class RequestFactory :  IRequestFactory
    {
        private readonly Func<HttpClient> _factory;
        private readonly Uri _requestUri;
        private readonly HttpVerb _httpVerb;

        private HttpClient GetClient()
        {
            return _factory();
        }

        public RequestFactory(Uri requestUri, HttpVerb httpVerb, Func<HttpClient> factory)
        {
            _requestUri = requestUri;
            _httpVerb = httpVerb;
            _factory = factory;
        }

        public RequestFactory(Uri requestUri, HttpVerb httpVerb)
            : this(requestUri, httpVerb, () => new HttpClient())
        {
        }

        public IRequestFactory AddAuthorizationHeader(Func<string> authBuilder)
        {
            Func<string, OAuthMessageHandler> getHandler = (authHeader) =>
            {
                var httpHandler = new HttpClientHandler();
                if (httpHandler.SupportsAutomaticDecompression)
                {
                    httpHandler.AutomaticDecompression =
                        DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }
                return new OAuthMessageHandler(authHeader, httpHandler);
            };
            return new RequestFactory(_requestUri, _httpVerb, () =>
            {
                var auth = authBuilder().Substring(6); // Skip "OAuth "

                var handler = getHandler(auth);
                
                return new HttpClient(handler);
            });
        }

        public async Task<ResponseData> GetResponseAsync()
        {
            using (var client = _factory())
            {
                var message = await client.GetAsync(_requestUri);
                var body = await message.Content.ReadAsStringAsync();
                return new ResponseData(
                    body,
                    message.StatusCode,
                    message.IsSuccessStatusCode,
                    message.RequestMessage.RequestUri);
            }
        }

        public async Task<ResponseData> GetPostResponseAsync(IEnumerable<QueryParameter> postForm)
        {
            using (var client = _factory())
            {
                var content = postForm == null
                    ? null
                    : new FormUrlEncodedContent(postForm.Select(q => q.ToKeyValuePair()));
                var message = await client.PostAsync(_requestUri, content);
                var body = await message.Content.ReadAsStringAsync();
                return new ResponseData(
                    body,
                    message.StatusCode,
                    message.IsSuccessStatusCode,
                    message.RequestMessage.RequestUri);
            }
        }

        public async Task<ResponseData> GetPostMultipartResponseAsync(IEnumerable<QueryParameter> postForm, UploadFile uploadFile)
        {
            using (var client = _factory())
            {
                var content = new MultipartFormDataContent();
                if (postForm != null)
                {
                    content.Add(new FormUrlEncodedContent(postForm.Select(q => q.ToKeyValuePair())));
                }
                if (uploadFile != null)
                {
                    content.Add(new StreamContent(uploadFile.Stream), uploadFile.FieldName, uploadFile.FileName);
                }
                var message = await client.PostAsync(_requestUri, content);
                var body = await message.Content.ReadAsStringAsync();
                return new ResponseData(
                    body,
                    message.StatusCode,
                    message.IsSuccessStatusCode,
                    message.RequestMessage.RequestUri);
            }
        }
    }
}
