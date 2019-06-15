using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandDotNet
{
    internal static class ArgumentParser
    {
        public static IEnumerable<string> SplitFlags(params string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.Length > 2 && IsShortOption(arg) && !HasValue(arg) )
                {
                    foreach (var flag in arg.ToCharArray().Skip(1))
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

        private static bool IsShortOption(string arg)
        {
            return arg[0] == '-' && arg[1] != '-';
        }

        private static bool HasValue(string arg)
        {
            return arg[2] == ':' || arg[2] == '=';
        }
    }
}