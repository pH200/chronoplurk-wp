using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;

namespace Plurto.Converters
{
    public class LimitedToConverter : JsonConverter
    {
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value.ToString();
                if (value == "|0|")
                {
                    return new int[] { 0 };
                }

                var splitted = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                var result = TryParseUsers(splitted);

                return result;
            }

            throw new JsonSerializationException("Expected string.");
        }

        private static IEnumerable<int> TryParseUsers(IEnumerable<string> splitted)
        {
            foreach (var s in splitted)
            {
                int userNumber;
                if (int.TryParse(s, out userNumber))
                {
                    yield return userNumber;
                }
            }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IEnumerable<int>);
        }
    }
}
