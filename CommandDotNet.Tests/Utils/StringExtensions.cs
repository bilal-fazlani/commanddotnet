using System;
using System.Linq;

namespace CommandDotNet.Tests.Utils
{
    public static class StringExtensions
    {
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
    }
}