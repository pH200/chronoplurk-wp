using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plurto.Core;

namespace Plurto
{
    internal static class Logging
    {
        public static void Log(string message)
        {
            if (Config.LoggingLevel != LoggingLevel.None)
            {
                WriteLine(message);
            }
        }

        public static void Log(string message, params object[] args)
        {
            if (Config.LoggingLevel != LoggingLevel.None)
            {
                WriteLine(message, args);
            }
        }

        public static void WriteLine(string message)
        {
            if (message == null)
            {
                Debug.WriteLine("");
                return;
            }
            if (Config.LoggingLinebreak <= 0)
            {
                Debug.WriteLine(message);
                return;
            }
            foreach (var output in message.SplitByLength(Config.LoggingLinebreak))
            {
                Debug.WriteLine(output);
            }
        }

        public static void WriteLine(string message, params object[] args)
        {
            if (message == null)
            {
                Debug.WriteLine("");
                return;
            }
            if (Config.LoggingLinebreak <= 0)
            {
                Debug.WriteLine(message, args);
                return;
            }

            WriteLine(String.Format(message, args));
        }

        private static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }
    }
}
