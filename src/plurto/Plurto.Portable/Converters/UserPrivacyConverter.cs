using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Plurto.Core;
using Plurto.Extensions;

namespace Plurto.Converters
{
    public class UserPrivacyConverter : JsonConverter
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
                    case "world":
                        return UserPrivacy.World;
                    case "only_friends":
                        return UserPrivacy.OnlyFriends;
                    case "only_me":
                        return UserPrivacy.OnlyMe;
                }
                return UserPrivacy.World;
            }

            throw new JsonSerializationException("Expected string.");
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UserPrivacy);
        }
    }
}
