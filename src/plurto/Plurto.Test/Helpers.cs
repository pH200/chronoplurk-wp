using System.Collections;
using System.Diagnostics;
using ServiceStack.Text;

namespace Plurto.Test
{
    public static class Helper
    {
        public static void Dump(IEnumerable collection)
        {
            if (collection == null)
            {
                return;
            }
            Trace.WriteLine(collection.Dump());
        }
    }

    public static class DumpExtensions
    {
        public static void WriteDump(this IEnumerable collection)
        {
            Helper.Dump(collection);
        }

        public static void WriteDump<T>(this T value)
        {
            Trace.WriteLine(value.Dump());
        }
    }
}
