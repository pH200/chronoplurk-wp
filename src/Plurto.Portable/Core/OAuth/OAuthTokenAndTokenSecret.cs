using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plurto.Core.OAuth
{
    public sealed class OAuthTokenAndTokenSecret
    {
        public string Token { get; private set; }

        public string TokenSecret { get; private set; }

        public OAuthTokenAndTokenSecret(string token, string tokenSecret)
        {
            Token = token;
            TokenSecret = tokenSecret;
        }

        public override string ToString()
        {
            return string.Format("Token: {0}, Token Secret: {1}", Token, TokenSecret);
        }
    }
}
