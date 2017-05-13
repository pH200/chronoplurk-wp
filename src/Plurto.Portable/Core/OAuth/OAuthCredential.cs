// -----------------------------------------------------------------------
// <copyright file="OAuthRequestData.cs" company="Ting-Yu Lin">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plurto.Core.OAuth
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class OAuthCredential
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public OAuthSignatureMethod SignatureMethod { get; set; }

        public string Realm { get; set; }
        public string Version { get; set; }
        public string Callback { get; set; }

        public string Verifier { get; set; }

        public string Signature { get; private set; }
        public string Timestamp { get; private set; }
        public string Nonce { get; private set; }

        public HttpVerb HttpVerb { get; set; }

        public bool EscapeSignature { get; set; }

        public IEnumerable<QueryParameter> QueryParameters { get; set; }        

        public OAuthCredential()
        {
            SignatureMethod = OAuthSignatureMethod.HMACSHA1;
            HttpVerb = HttpVerb.Get;
            EscapeSignature = true;
            Version = "1.0";
            Realm = "";
        }

        public OAuthCredential ShallowCopy()
        {
            return (OAuthCredential)this.MemberwiseClone();
        }

        public void SetTimestampNow()
        {
            Timestamp = DateTime.Now.ToTimestamp().ToString().Substring(0, 10);
        }

        public void SetRandomNonce()
        {
            Nonce = Guid.NewGuid().ToString("N");
        }

        public void GenerateTimestampAndNonce()
        {
            SetTimestampNow();
            SetRandomNonce();
        }

        public void GenerateSignature(Func<string, string, string> hmacsha1, Uri requestUri)
        {
            Signature = OAuthSignatureGenerator.GenerateSignature(hmacsha1, this, requestUri);
        }
    }
}
