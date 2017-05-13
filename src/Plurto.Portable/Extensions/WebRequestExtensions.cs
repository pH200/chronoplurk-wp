using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Plurto.Extensions
{
    internal static class WebRequestExtensions
    {
        /// <summary>
        /// Sets content length for WebRequest. Has no effect on Silverlight.
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="value"></param>
        public static void SetContentLength(this WebRequest webRequest, long value)
        {
#if CLR
            webRequest.ContentLength = value;
#endif
        }
    }
}
