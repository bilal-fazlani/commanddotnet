using System;
using Humanizer;

namespace CommandDotNet
{
    internal static class StringExtensions
    {
        public static string ChangeCase(this string value, Case @case)
        {
            switch (@case)
            {
                case Case.DontChange:
                    return value;
                case Case.CamelCase:
                    return value.Camelize();
                case Case.PascalCase:
                    return value.Dehumanize();
                case Case.KebabCase:
                    return value.Kebaberize();
                case Case.LowerCase:
                    return value.ToLowerInvariant();
                default:
                    return value;
            }
        }

        internal static bool IsNullOrEmpty(this string value) => 
            string.IsNullOrEmpty(value);

        internal static bool IsNullOrWhitespace(this string value) => 
            string.IsNullOrWhiteSpace(value);

        internal static string UnlessNullOrWhitespace(this string value, Func<string, string> map = null) =>
            value.IsNullOrWhitespace()
                ? null
                : map == null
                    ? value
                    : map(value);
    }
}