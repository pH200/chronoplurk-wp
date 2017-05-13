using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Plurto.Core
{
    public class PlurtoPlatform
    {
        public static string ConvertHMACSHA1(string key, string buffer)
        {
            var algorithm = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);
            var keyBuffer = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var hmacKey = algorithm.CreateKey(keyBuffer);
            var dataBuffer = CryptographicBuffer.ConvertStringToBinary(buffer, BinaryStringEncoding.Utf8);
            var hash = CryptographicEngine.Sign(hmacKey, dataBuffer);
            return CryptographicBuffer.EncodeToBase64String(hash);
        }

        public static IRequestFactory CreateRequestFactory(Uri uri, HttpVerb httpVerb)
        {
            return new RequestFactory(uri, httpVerb);
        }
    }
}
