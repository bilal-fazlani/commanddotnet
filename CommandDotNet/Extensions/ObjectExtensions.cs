using System;
using System.Collections;
using System.Linq;

namespace CommandDotNet.Extensions
{
    internal static class ObjectExtensions
    {
        internal static bool IsNullValue(this object obj)
        {
            return obj == null || obj == DBNull.Value;
        }

        /// <summary></summary>
        internal static string ValueToString(this object value)
        {
            if (value.IsNullValue())
            {
                return null;
            }

            if (value is string str)
            {
                return str;
            }

            if (value is IEnumerable collection)
            {
                return collection.Cast<object>().Select(i => i?.ToString()).ToCsv();
            }

            return value.ToString();
        }
    }
}