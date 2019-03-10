using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandDotNet
{
    internal class ArgumentParser
    {
        private static readonly Regex ArgumentsSplitRegex = new Regex(@"(?<!\\)"".+(?<!\\)""|(?<!\\)'.+(?<!\\)'|[^\s]+", RegexOptions.Compiled);
        private static readonly Regex TrimRegex = new Regex(@"^""|^'|""$|'$", RegexOptions.Compiled);
        private static readonly Regex UnescapeRegex = new Regex(@"\\'|\\""", RegexOptions.Compiled);
        private static readonly Regex MixedFlagsRegex = new Regex(@"(?<=^-)\w{2,}", RegexOptions.Compiled);

        public static IEnumerable<string> SplitFlags(params string[] args)
        {
            var joined = string.Join(" ", args);
            foreach (Match match in ArgumentsSplitRegex.Matches(joined))
            {
                var sanitized = SanitizeValue(match.Value);
                var mixedMatch = MixedFlagsRegex.Match(sanitized);
                if (mixedMatch.Success)
                {
                    foreach (var @char in mixedMatch.Value.ToCharArray())
                    {
                        yield return new string(new[] {'-', @char});
                    }
                }
                else
                {
                    yield return sanitized;
                }
            }
        }

        private static string SanitizeValue(string value)
        {
            return UnescapeRegex.Replace(
                TrimRegex.Replace(
                    value, 
                    string.Empty), 
                string.Empty);
        }
    }
}