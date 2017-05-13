// -----------------------------------------------------------------------
// <copyright file="OAuthAuthorizationHeaderBuilder.cs" company="Ting-Yu Lin">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plurto.Extensions;

namespace Plurto.Core.OAuth
{
    /// <summary>
    /// Authorization header builder for OAuth.
    /// </summary>
    public static class OAuthAuthorizationHeaderBuilder
    {
        public static string Build(OAuthCredential oAuthCredential, bool addScheme=true)
        {
            var authDataDict = GenerateAuthorizationDict(oAuthCredential);
            var sb = new StringBuilder(addScheme ? "OAuth " : "");
            foreach (var data in authDataDict.Where(d => d.Value != null))
            {
                sb.Append(data.Key);
                sb.Append("=\"");
                sb.Append(data.Value);
                sb.Append("\",");
            }
            return sb.ToStringSkipLast();
        }

        private static IEnumerable<KeyValuePair<string, string>> GenerateAuthorizationDict(OAuthCredential data)
        {
            var dict = new[]
            {
                new KeyValuePair<string, string>(OAuthKey.Realm, data.Realm),
                new KeyValuePair<string, string>(OAuthKey.Version, data.Version),
                new KeyValuePair<string, string>(OAuthKey.ConsumerKey, data.ConsumerKey),
                new KeyValuePair<string, string>(OAuthKey.SignatureMethod, data.SignatureMethod.ToKey()),
                new KeyValuePair<string, string>(OAuthKey.Signature, data.Signature),
                new KeyValuePair<string, string>(OAuthKey.Timestamp, data.Timestamp),
                new KeyValuePair<string, string>(OAuthKey.Nonce, data.Nonce),
                new KeyValuePair<string, string>(OAuthKey.Token, data.Token),
                new KeyValuePair<string, string>(OAuthKey.Verifier, data.Verifier),
            };
            return dict;
        }
    }
}
