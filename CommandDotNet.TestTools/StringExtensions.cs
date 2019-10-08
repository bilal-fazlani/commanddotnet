using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Tokens;

namespace CommandDotNet.TestTools
{
    public static class StringExtensions
    {
        /// <summary>
        /// Trims white space from all lines to ensure consistent results for test assertions.<br/>
        /// This is to deal with whitespace padding when some lines don't need all elements.
        /// </summary>
        public static string NormalizeLineEndings(this string text)
        {
            // split text and trim white space from all lines
            var lines = text
                .SplitIntoLines()
                .Select(l => l.TrimEnd());

            // join with a consistent line ending
            var result = string.Join(Environment.NewLine, lines);

            // trim extra empty line endings so tests are easier to write with less wasted space.
            return result.TrimEnd(Environment.NewLine.ToCharArray());
        }

        /// <summary>Split the arguments using <see cref="CommandLineStringSplitter"/></summary>
        public static string[] SplitArgs(this string args)
        {
            return args == null
                ? new string[0]
                : CommandLineStringSplitter.Instance.Split(args).ToArray();
        }

        public static IEnumerable<ConsoleKeyInfo> ToConsoleKeyInfos(this string text)
        {
            // "\r\n" would result in two ConsoleKey.Enter
            return text.Replace("\r\n", "\r").Select(ToConsoleKeyInfo);
        }

        public static ConsoleKeyInfo ToConsoleKeyInfo(this char c)
        {
            return new ConsoleKeyInfo(c, c.ToConsoleKey(), false, false, false);
        }

        public static ConsoleKey ToConsoleKey(this char c)
        {
            switch (c)
            {
                case ' ':
                    return ConsoleKey.Spacebar;
                case '+':
                    return ConsoleKey.Add;
                case '-':
                    return ConsoleKey.Subtract;
                case '*':
                    return ConsoleKey.Multiply;
                case '/':
                    return ConsoleKey.Divide;
                case '\b':
                    return ConsoleKey.Backspace;
                case '\t':
                    return ConsoleKey.Tab;
                case '\n':
                    return ConsoleKey.Enter;
                case '\r':
                    return ConsoleKey.Enter;
                default:
                    return char.IsNumber(c)
                    ? $"D{c}".ParseEnum<ConsoleKey>(true)
                    : c.ToString().TryParseEnum<ConsoleKey>(out var result, true)
                        ? result
                        : ConsoleKey.Oem1; // any Oem would do
            }
        }

        internal static T ParseEnum<T>(this string text, bool ignoreCase = false) =>
            (T) Enum.Parse(typeof(T), text, ignoreCase);

        internal static bool TryParseEnum<T>(this string text, out T result, bool ignoreCase = false) where T : struct =>
            Enum.TryParse<T>(text, ignoreCase, out result);
    }
}