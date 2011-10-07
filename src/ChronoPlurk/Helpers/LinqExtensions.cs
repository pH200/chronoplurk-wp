using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return (source == null || source.IsEmpty());
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return (!source.Any());
        }
    }
}
