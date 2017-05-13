using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Plurto.Entities;

namespace Plurto.Converters
{
    public class EmptyArrayOrUsersConverter : CustomCreationConverter<IDictionary<int, User>>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        return null;
                    }
                }
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override IDictionary<int, User> Create(Type objectType)
        {
            return new Dictionary<int, User>();
        }
    }
}
