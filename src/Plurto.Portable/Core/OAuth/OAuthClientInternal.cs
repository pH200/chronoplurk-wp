// -----------------------------------------------------------------------
// <copyright file="OAuthClient.cs" company="Ting-Yu Lin">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plurto.Core.OAuth
{
    public class OAuthClientInternal : IRequestClient
    {
        private const string RequestTokenEndpoint = "https://www.plurk.com/OAuth/request_token";
        private const string RequestTokenEndpointSecure = "https://www.plurk.com/OAuth/request_token";
        private const string AuthorizationPage = "https://www.plurk.com/OAuth/authorize";
        private const string AuthorizationPageMobile = "https://www.plurk.com/m/authorize";
        private const string AccessTokenEndpoint = "https://www.plurk.com/OAuth/access_token";
        private const string AccessTokenEndpointSecure = "https://www.plurk.com/OAuth/access_token";
        private const string ApiEndpoint = "https://www.plurk.com/APP";
        private const string ApiEndpointSecure = "https://www.plurk.com/APP";

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string Token { get; set; }

        public string TokenSecret { get; set; }

        public bool IsAlwaysSecure { get; set; }

        /// <summary>
        /// <para>Method for WebRequest. You will be unable to upload file if this is set to GET.</para>
        /// <para>Default is POST.</para>
        /// </summary>
        public HttpVerb HttpVerb { get; set; }

        /// <summary>
        /// Platform implementation of HMACSHA1 algorithm.
        /// </summary>
        private readonly Func<string, string, string> _hmacsha1;
        private readonly Func<Uri, HttpVerb, IRequestFactory> _requestFactory;

        public OAuthClientInternal(Func<string, string, string> hmacsha1, Func<Uri, HttpVerb, IRequestFactory> requestFactory)
        {
            if (hmacsha1 == null)
            {
                throw new ArgumentNullException("hmacsha1");
            }
            if (requestFactory == null)
            {
                throw new ArgumentNullException("hmacsha1");
            }
            _hmacsha1 = hmacsha1;
            _requestFactory = requestFactory;
            HttpVerb = HttpVerb.Post;
        }

        public void SetToken(OAuthTokenAndTokenSecret token)
        {
            Token = token.Token;
            TokenSecret = token.TokenSecret;
        }

        public void FlushToken()
        {
            Token = null;
            TokenSecret = null;
        }

        public Uri GetAuthorizationUri(bool mobile = false)
        {
            if (string.IsNullOrEmpty(Token))
            {
                throw new InvalidOperationException("Set request token before opening authorization url.");
            }
            var page = mobile ? AuthorizationPageMobile : AuthorizationPage;
            return new Uri(page + "?oauth_token=" + Token, UriKind.Absolute);
        }

        public async Task<OAuthTokenAndTokenSecret> ObtainRequestTokenAsync(bool secure = true)
        {
            var url = (secure && !Config.AlwaysDisableHttps) ? RequestTokenEndpointSecure : RequestTokenEndpoint;
            var queryUri = new Uri(url, UriKind.Absolute);

            Func<string> authBuilder = () =>
            {
                var oAuthCredential = new OAuthCredential()
                {
                    HttpVerb = HttpVerb.Post,
                    ConsumerKey = ConsumerKey,
                    ConsumerSecret = ConsumerSecret,
                };
                oAuthCredential.GenerateTimestampAndNonce();
                oAuthCredential.GenerateSignature(_hmacsha1, queryUri);
                return OAuthAuthorizationHeaderBuilder.Build(oAuthCredential);
            };
            var request = _requestFactory(queryUri, HttpVerb.Post).AddAuthorizationHeader(authBuilder);

            var response = await request.GetPostResponseAsync(null);
            var token = ConvertQueryParametersToToken(SimpleParseQueryString(response.Body));
            this.SetToken(token);
            return token;
        }

        public async Task<OAuthTokenAndTokenSecret> ObtainAccessTokenAsync(string verifier, bool secure = true)
        {
            var url = (secure && !Config.AlwaysDisableHttps) ? AccessTokenEndpointSecure : AccessTokenEndpoint;
            var queryUri = new Uri(url, UriKind.Absolute);

            Func<string> authBuilder = () =>
            {
                var oAuthCredential = new OAuthCredential()
                {
                    HttpVerb = HttpVerb.Post,
                    ConsumerKey = ConsumerKey,
                    ConsumerSecret = ConsumerSecret,
                    Token = Token,
                    TokenSecret = TokenSecret,
                    Verifier = verifier,
                };
                oAuthCredential.GenerateTimestampAndNonce();
                oAuthCredential.GenerateSignature(_hmacsha1, queryUri);
                return OAuthAuthorizationHeaderBuilder.Build(oAuthCredential);
            };

            var request = _requestFactory(queryUri, HttpVerb.Post).AddAuthorizationHeader(authBuilder);

            var response = await request.GetPostResponseAsync(null);
            var token = ConvertQueryParametersToToken(SimpleParseQueryString(response.Body));
            this.SetToken(token);
            return token;
        }

        private static IEnumerable<QueryParameter> SimpleParseQueryString(string query)
        {
            string key = null;
            var anchor = 0;
            for (int i = 0; i < query.Length; i++)
            {
                switch (query[i])
                {
                    case '=':
                        key = query.Substring(anchor, i - anchor);
                        anchor = i + 1;
                        break;
                    case '&':
                        var value = query.Substring(anchor, i - anchor);
                        anchor = i + 1;
                        if (key != null)
                        {
                            yield return new QueryParameter(key, value);
                            key = null;
                        }
                        break;
                }
            }
            if(key != null)
            {
                var value = query.Substring(anchor);
                yield return new QueryParameter(key, value);
            }
        }

        private static OAuthTokenAndTokenSecret ConvertQueryParametersToToken(IEnumerable<QueryParameter> parameters)
        {
            string tokenSecret = null;
            string token = null;
            
            foreach (var parameter in parameters)
            {
                switch (parameter.Key)
                {
                    case "oauth_token_secret":
                        tokenSecret = parameter.Value;
                        break;
                    case "oauth_token":
                        token = parameter.Value;
                        break;
                }
            }

            if (token != null && tokenSecret != null)
            {
                return new OAuthTokenAndTokenSecret(token, tokenSecret);
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseData> GetResponseAsync(HttpVerb httpVerb, bool? secure, string method, IEnumerable<QueryParameter> parameters, UploadFile uploadFile)
        {
            if (HttpVerb == HttpVerb.Get && uploadFile != null)
            {
                throw new InvalidOperationException("Use HTTP Post for file uploading.");
            }

            var oauthHttpVerb = HttpVerb; // httpVerb argument is for legacy compatibility.

            var queryParameters = parameters;
            var queryUri = UrlBuilder(oauthHttpVerb, secure, method, queryParameters);

            Func<string> authBuilder = () =>
            {
                var oAuthCredential = new OAuthCredential()
                {
                    HttpVerb = oauthHttpVerb,
                    QueryParameters = parameters,
                    ConsumerKey = ConsumerKey,
                    ConsumerSecret = ConsumerSecret,
                    Token = Token,
                    TokenSecret = TokenSecret,
                };
                oAuthCredential.GenerateTimestampAndNonce();
                oAuthCredential.GenerateSignature(_hmacsha1, queryUri);
                return OAuthAuthorizationHeaderBuilder.Build(oAuthCredential);
            };
            
            var request = _requestFactory(queryUri, oauthHttpVerb).AddAuthorizationHeader(authBuilder);

            if (oauthHttpVerb == HttpVerb.Get)
            {
                return await request.GetResponseAsync();
            }
            else
            {
                if (uploadFile == null)
                {
                    return await request.GetPostResponseAsync(queryParameters);
                }
                else
                {
                    return await request.GetPostMultipartResponseAsync(queryParameters, uploadFile);
                }
            }
        }

        private Uri UrlBuilder(HttpVerb httpVerb, bool? secure, string method, IEnumerable<QueryParameter> parameters)
        {
            var requestSecure = ((secure ?? IsAlwaysSecure) && !Config.AlwaysDisableHttps);
            var urlBuilder = requestSecure
                                 ? new StringBuilder(ApiEndpointSecure, 400)
                                 : new StringBuilder(ApiEndpoint, 400);
            urlBuilder.Append(method);
            var url = httpVerb == HttpVerb.Get
                       ? BuildGetUrl(urlBuilder, parameters)
                       : urlBuilder.ToString();
            return new Uri(url, UriKind.Absolute);
        }

        private static string BuildGetUrl(StringBuilder urlBuilder, IEnumerable<QueryParameter> parameters)
        {
            var isFirstAppend = true;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    if (isFirstAppend)
                    {
                        urlBuilder.Append('?');
                        isFirstAppend = false;
                    }
                    else
                    {
                        urlBuilder.Append('&');
                    }
                    urlBuilder.Append(parameter.Key);
                    urlBuilder.Append('=');
                    urlBuilder.Append(HttpTools.EscapeDataStringOmitNull(parameter.Value));
                }
            }
            return urlBuilder.ToString();
        }

        public async Task<ResponseData> Tester(OAuthCredential oAuthCredential,  Uri queryUri)
        {
            var httpVerb = oAuthCredential.HttpVerb;
            var requestUri = queryUri;
            Func<string> authBuilder = () =>
            {
                var cred = oAuthCredential.ShallowCopy();
                cred.GenerateTimestampAndNonce();
                cred.GenerateSignature(_hmacsha1, requestUri);
                return OAuthAuthorizationHeaderBuilder.Build(cred);
            };
            var request = _requestFactory(requestUri, httpVerb).AddAuthorizationHeader(authBuilder);

            return await (httpVerb == HttpVerb.Get
                       ? request.GetResponseAsync()
                       : request.GetPostResponseAsync(oAuthCredential.QueryParameters));
        }
    }
}
