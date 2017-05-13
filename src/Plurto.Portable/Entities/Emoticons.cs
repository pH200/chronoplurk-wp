using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Plurto.Core;

namespace Plurto.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Emoticons
    {
        [JsonProperty("karma")]
        public IDictionary<string, string[][]> Karma { get; set; }

        [JsonProperty("recuited")]
        public IDictionary<string, string[][]> Recuited { get; set; }

        [JsonProperty("custom")]
        [NotAvailableOnLegacy]
        public string[][] Custom { get; set; }

        public IDictionary<string, string> GetKarmaEmoticons(double? karma = null)
        {
            return GetEmoticonsInternal(Karma, karma);
        }

        public IDictionary<string, string> GetRecuitedEmoticons(double? recuited = null)
        {
            return GetEmoticonsInternal(Recuited, recuited);
        }

        [NotAvailableOnLegacy]
        public IDictionary<string, string> GetCustomEmotions()
        {
            return Custom
                .Where(array => array.Length >= 2)
                .ToDictionary(array => array[0], array => array[1]);
        }

        [NotAvailableOnLegacy]
        public IDictionary<string, string> GetCustomBracktedEmoticons()
        {
            return Custom
                .Where(array => array.Length >= 2)
                .ToDictionary(array => "[" + array[0] + "]",
                              array => array[1]);
        }

        private static IDictionary<string, string> GetEmoticonsInternal(IDictionary<string, string[][]> dict, double? value = null)
        {
            var list = value.HasValue
                           ? GetComparedList(dict, value.Value)
                           : dict.Values;
            var emoticons = from outerArrays in list
                            from array in outerArrays
                            where array.Length >= 2
                            select array;
            return emoticons.ToDictionary(array => array[0], array => array[1]);
        }
        
        private static IEnumerable<string[][]> GetComparedList(IDictionary<string, string[][]> dict, double value)
        {
            foreach (var pair in dict)
            {
                double listValue;
                if (double.TryParse(pair.Key, out listValue))
                {
                    if (value >= listValue)
                    {
                        yield return pair.Value;
                    }
                }
            }
        }
    }
}
