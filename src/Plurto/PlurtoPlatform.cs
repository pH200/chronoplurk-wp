using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plurto.Core
{
    public class PlurtoPlatform
    {
        public static string ConvertHMACSHA1(string key, string buffer)
        {
            var hashAlgorithm = new System.Security.Cryptography.HMACSHA1();
            hashAlgorithm.Key = Encoding.UTF8.GetBytes(key);
            var bytes = Encoding.UTF8.GetBytes(buffer);
            var hash = hashAlgorithm.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static IRequestFactory CreateRequestFactory(Uri uri, HttpVerb httpVerb)
        {
            return new RequestFactory(uri, httpVerb);
        }
    }
}
