using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plurto.Helpers
{
    public static class TryParse
    {
        public static long? Long(string s)
        {
            long value;
            if (long.TryParse(s, out value))
            {
                return value;
            }
            return null;
        }
    }
}
