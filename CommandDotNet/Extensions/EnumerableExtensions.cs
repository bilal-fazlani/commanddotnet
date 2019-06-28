using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Extensions
{
    internal static class EnumerableExtensions
    {
        public static string ToCsv(this IEnumerable<string> items, string separator = ",")
        {
            return string.Join(separator, items);
        }

        public static string ToCsv<T>(this IEnumerable<T> items, string separator = ",")
        {
            return items.Select(i => i?.ToString()).ToCsv(separator);
        }

        internal static string ToCsv(this IEnumerable items, string separator = ",")
        {
            return items.Cast<object>().ToCsv(separator);
        }

        /// <summary>Joins the string into a sorted delimited string. Useful for logging.</summary>
        public static string ToOrderedCsv(this IEnumerable<string> items, string separator = ",")
        {
            return items.OrderBy(i => i).ToCsv(separator);
        }

        /// <summary>Joins the object.ToString() into a sorted delimited string. Useful for logging.</summary>
        public static string ToOrderedCsv<T>(this IEnumerable<T> items, string separator = ",")
        {
            return items.Select(i => i?.ToString()).ToOrderedCsv(separator);
        }

        internal static string ToOrderedCsv(this IEnumerable items, string separator = ",")
        {
            return items.Cast<object>().ToOrderedCsv();
        }

        internal static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}