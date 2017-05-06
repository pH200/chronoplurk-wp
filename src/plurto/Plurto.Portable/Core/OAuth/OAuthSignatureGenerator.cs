// -----------------------------------------------------------------------
// <copyright file="OAuthSignatureGenerator.cs" company="Ting-Yu Lin">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plurto.Extensions;
using System.Threading.Tasks;

namespace Plurto.Core.OAuth
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class OAuthSignatureGenerator
    {
        public static string GenerateSignature(Func<string, string, string> hmacsha1, OAuthCredential oAuthCredential, Uri requestUri)
        {
            var consumerSecret = oAuthCredential.ConsumerSecret;
            var tokenSecret = oAuthCredential.TokenSecret;

            string signature;
            switch (oAuthCredential.SignatureMethod)
            {
                case OAuthSignatureMethod.HMACSHA1:
                    var key = consumerSecret + "&" + tokenSecret;
                    var signatureBase = GenerateSignatureBase(oAuthCredential, requestUri);
                    signature = hmacsha1(key, signatureBase);
                    break;
                default:
                    throw new NotSupportedException("Not supported signature method.");
            }

            if (oAuthCredential.EscapeSignature)
            {
                signature = HttpTools.EscapeDataStringOmitNull(signature);
            }
            return signature;
        }

        private static string GenerateSignatureBase(OAuthCredential oAuthCredential, Uri requestUri)
        {
            var parameters = new List<QueryParameter>();

            if (oAuthCredential.QueryParameters != null)
            {
                parameters.AddRange(oAuthCredential.QueryParameters);
            }

            parameters.Add(new QueryParameter(OAuthKey.ConsumerKey, oAuthCredential.ConsumerKey));
            parameters.Add(new QueryParameter(OAuthKey.SignatureMethod, oAuthCredential.SignatureMethod.ToKey()));
            parameters.Add(new QueryParameter(OAuthKey.Timestamp, oAuthCredential.Timestamp));
            parameters.Add(new QueryParameter(OAuthKey.Nonce, oAuthCredential.Nonce));

            if (!oAuthCredential.Version.IsNullOrEmpty())
            {
                parameters.Add(new QueryParameter(OAuthKey.Version, oAuthCredential.Version));
            }
            if (!oAuthCredential.Token.IsNullOrEmpty())
            {
                parameters.Add(new QueryParameter(OAuthKey.Token, oAuthCredential.Token));
            }
            if (!oAuthCredential.Verifier.IsNullOrEmpty())
            {
                parameters.Add(new QueryParameter(OAuthKey.Verifier, oAuthCredential.Verifier));
            }

            var normalizedRequestParameters = NormalizeRequestParameters(parameters);
            var normalizedUrl = NormalizeRequestUrl(requestUri);
            var sb = new StringBuilder();
            sb.Append(oAuthCredential.HttpVerb.ToKey());
            sb.Append('&');
            sb.Append(HttpTools.EscapeDataStringOmitNull(normalizedUrl));
            sb.Append('&');
            sb.Append(HttpTools.EscapeDataStringOmitNull(normalizedRequestParameters));

            return sb.ToString();
        }

        private static string NormalizeRequestParameters(IEnumerable<QueryParameter> parameters)
        {
            var orderdParameters = parameters.OrderBy(p => p.Key);
            var sb = new StringBuilder();
            foreach (var parameter in orderdParameters)
            {
                sb.Append(parameter.Key);
                sb.Append('=');
                sb.Append(HttpTools.EscapeDataStringOmitNull(parameter.Value));
                sb.Append('&');
            }
            return sb.ToStringSkipLast();
        }

        private static string NormalizeRequestUrl(Uri uri)
        {
            var normalizedUrl = uri.Scheme + "://" + uri.Host;
            if (!((uri.Scheme == "http" && uri.Port == 80) || (uri.Scheme == "https" && uri.Port == 443)))
            {
                normalizedUrl += ":" + uri.Port;
            }
            return normalizedUrl + uri.AbsolutePath;
        }
    }
}
