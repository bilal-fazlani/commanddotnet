using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandDotNet.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static string ToCsv<T>(this IEnumerable<T> items, string separator = ",")
        {
            return string.Join(separator, items);
        }

        /// <summary>Joins the object.ToString() into a sorted delimited string. Useful for logging.</summary>
        public static string ToOrderedCsv<T>(this IEnumerable<T> enumerable, string separator = ",")
        {
            return enumerable
                .Select(i => i?.ToString())
                .ToOrderedCsv(separator);
        }

        /// <summary>Joins the string into a sorted delimited string. Useful for logging.</summary>
        public static string ToOrderedCsv(this IEnumerable<string> enumerable, string separator = ",")
        {
            return enumerable.OrderBy(i => i).ToCsv(separator);
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