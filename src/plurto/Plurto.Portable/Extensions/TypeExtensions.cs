using System;
using System.Reflection;

namespace Plurto.Extensions
{
    internal static class TypeExtensions
    {
        public static Type GetUnderlying(this Type type)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(type);
            }
            return type;
        }
        public static bool IsNullable(this Type type)
        {
            return (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
