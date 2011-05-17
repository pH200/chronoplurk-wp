using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Linq
{
    public static class LinqExtensions
    {
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
