using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Plurto.Converters
{
    /// <summary>
    /// Note: CanConvert works differently on different platforms for covariance and contravariace.
    /// </summary>
    /// <typeparam name="T">No meanings.</typeparam>
    public class IgnoreEmptyArrayConverter<T> : CustomCreationConverter<T>
        where T : class
    {
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
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

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        public override T Create(Type objectType)
        {
            return Activator.CreateInstance<T>();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        public override bool CanConvert(Type objectType)
        {
#if CLR
            return objectType.IsAssignableFrom(typeof (T)) || base.CanConvert(objectType);
#else
            return objectType.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) || base.CanConvert(objectType);
#endif
        }
    }
}
