using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plurto.Entities
{
    public sealed class ReplurkSuccess
    {
        public bool Success { get; set; }

        public IEnumerable<KeyValuePair<long, ReplurkResult>> Results { get; set; }

        // NOTE: "plurk" property is also available but undocumented.
        public class ReplurkResult
        {
            public bool Success { get; set; }

            public string Error { get; set; }
        }
    }
}
