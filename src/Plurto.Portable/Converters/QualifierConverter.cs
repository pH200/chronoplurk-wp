using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Plurto.Core;
using Plurto.Extensions;

namespace Plurto.Converters
{
    public class QualifierConverter : JsonConverter
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
                if (!objectType.IsNullable())
                {
                    throw new JsonSerializationException(string.Format("Cannot convert null value to {0}.", objectType));
                }
                return null;
            }
            if (reader.TokenType == JsonToken.String)
            {
                var value = reader.Value.ToString();
                switch (value)
                {
                    case "loves":
                        return Qualifier.Loves;
                    case "likes":
                        return Qualifier.Likes;
                    case "shares":
                        return Qualifier.Shares;
                    case "gives":
                        return Qualifier.Gives;
                    case "hates":
                        return Qualifier.Hates;
                    case "wants":
                        return Qualifier.Wants;
                    case "has":
                        return Qualifier.Has;
                    case "will":
                        return Qualifier.Will;
                    case "asks":
                        return Qualifier.Asks;
                    case "wishes":
                        return Qualifier.Wishes;
                    case "was":
                        return Qualifier.Was;
                    case "feels":
                        return Qualifier.Feels;
                    case "thinks":
                        return Qualifier.Thinks;
                    case "says":
                        return Qualifier.Says;
                    case "is":
                        return Qualifier.Is;
                    case ":":
                        return Qualifier.FreestyleColon;
                    case "freestyle":
                        return Qualifier.Freestyle;
                    case "hopes":
                        return Qualifier.Hopes;
                    case "needs":
                        return Qualifier.Needs;
                    case "wonders":
                        return Qualifier.Wonders;
                }

                return Qualifier.Freestyle;
            }

            throw new JsonSerializationException("Expected string.");
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Qualifier);
        }
    }
}
