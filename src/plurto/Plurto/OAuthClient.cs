using Plurto.Core;
using Plurto.Core.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plurto.Core.OAuth
{
    public class OAuthClient : OAuthClientInternal, IRequestClient
    {
        public OAuthClient()
            : base(PlurtoPlatform.ConvertHMACSHA1, PlurtoPlatform.CreateRequestFactory)
        {
        }

        public OAuthClient(string consumerKey, string consumerSecret)
            : this()
        {
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
        }

        public OAuthClient(string consumerKey, string consumerSecret, string token, string tokenSecret)
            : this()
        {
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
            this.Token = token;
            this.TokenSecret = tokenSecret;
        }
    }
}
