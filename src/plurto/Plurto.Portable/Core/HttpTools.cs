// -----------------------------------------------------------------------
// <copyright file="HttpTools.cs" company="Ting-Yu Lin">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Plurto.Extensions;

namespace Plurto.Core
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class HttpTools
    {
        public const string UnreservedCharacters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_.~";

        private static readonly KeyValuePair<string, string>[] UriRfc3986CharsToEscape = new[]
        {
            new KeyValuePair<string, string>("!", PercentEncode("!")),
            new KeyValuePair<string, string>("*", PercentEncode("*")),
            new KeyValuePair<string, string>("'", PercentEncode("'")),
            new KeyValuePair<string, string>("(", PercentEncode("(")),
            new KeyValuePair<string, string>(")", PercentEncode(")"))
        };

        public static string EscapeDataStringOmitNull(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return "";
            }
            else
            {
                var escaped = Uri.EscapeDataString(value);
                var entity = EscapeUriDataStringRfc3986(escaped);
                return entity;
            }
        }

        /// <summary>
        /// Plurk OAuth requires RFC3986 compliant URI.
        /// </summary>
        /// <param name="value">Escaped data string</param>
        /// <returns>RFC3986 escaped string</returns>
        private static string EscapeUriDataStringRfc3986(string value)
        {
            var escaped = new StringBuilder(value);
            foreach (var entity in UriRfc3986CharsToEscape)
            {
                escaped.Replace(entity.Key, entity.Value);
            }
            return escaped.ToString();
        }

        /// <summary>
        /// [RFC3986] Encoding. *WARNING* Do not use with multi-byte words.
        /// </summary>
        /// <param name="value"></param>
        public static string PercentEncode(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return "";
            }

            var sb = new StringBuilder();
            foreach (var ch in value)
            {
#if CLR
                if (UnreservedCharacters.Contains(ch))
#else
                if (UnreservedCharacters.Contains(ch.ToString()))
#endif
                {
                    sb.Append(ch);
                }
                else
                {
                    sb.Append('%' + string.Format("{0:X2}", (int) ch));
                }
            }
            return sb.ToString();
        }
    }
}
