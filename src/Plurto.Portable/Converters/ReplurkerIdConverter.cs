using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;

namespace Plurto.Converters
{
    /// <summary>
    /// Convert replurker_id correctly. SearchResult might return null, "" or integer replurker id.
    /// </summary>
    public class ReplurkerIdConverter : JsonConverter
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
                var value = (string)reader.Value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }
                int intValue;
                if (int.TryParse(value, out intValue))
                {
                    return (int?)intValue;
                }
                return null;
            }
            if (reader.TokenType == JsonToken.Integer)
            {
                int? value = (int)((long)reader.Value);
                return value;
            }

            throw new JsonSerializationException("Expected Integer.");
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int?);
        }
    }
}
