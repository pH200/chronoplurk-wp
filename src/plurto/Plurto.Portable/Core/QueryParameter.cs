// -----------------------------------------------------------------------
// <copyright file="QueryParameter.cs" company="Ting-Yu Lin">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Plurto.Core
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QueryParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public QueryParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public KeyValuePair<string, string> ToKeyValuePair()
        {
            return new KeyValuePair<string, string>(Key, Value);
        }
    }
}
