using System;
using System.Linq;

namespace Plurto.Helpers
{
    public static class Base36
    {
        private const string Table = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Decode Base36 value.
        /// </summary>
        public static long Decode(string value)
        {
            var iterable = value.ToLowerInvariant().ToCharArray().Reverse().Select((ch, index) => new { ch, index });
            var result = iterable.Sum(pair => Table.IndexOf(pair.ch) * (long)Math.Pow(36, pair.index));
            return result;
        }

        /// <summary>
        /// Encode number to Base36 string.
        /// </summary>
        public static string Encode(long value)
        {
            if (value <= 0)
            {
                return "0";
            }

            var divide = value;
            var result = ""; // Use string for short value.
            while (divide != 0)
            {
                result = Table[(int)(divide % 36)] + result;
                divide = divide / 36;
            }

            return result;
        }
    }
}
