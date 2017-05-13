// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Ting-Yu Lin">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


using System;
using System.Text;

namespace Plurto.Extensions
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal static class StringExtensions
    {
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string ToStringSkipLast(this StringBuilder stringBuilder)
        {
            return stringBuilder.ToString(0, Math.Max(0, stringBuilder.Length - 1));
        }
    }
}
