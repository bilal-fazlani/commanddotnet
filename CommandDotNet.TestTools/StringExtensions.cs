using System;
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
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
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
    }
}