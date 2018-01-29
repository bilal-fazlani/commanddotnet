using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandDotNet
{
    internal class ArgumentParser
    {
        public static IEnumerable<string> SplitFlags(params string[] args)
        {
            foreach (var arg in args)
            {
                Regex mixedFlagsRegex = new Regex(@"^^-(\w{2,})");
                Match match = mixedFlagsRegex.Match(arg);
                if (match.Success)
                {
                    List<char> flags = match.Groups[1].Value.ToList();
                    foreach (var flag in flags)
                    {
                        yield return $"-{flag}";
                    }
                }
                else
                {
                    yield return arg;
                }
            }
        }
    }
}