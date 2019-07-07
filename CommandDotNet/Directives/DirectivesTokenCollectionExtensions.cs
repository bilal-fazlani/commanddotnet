using System;
using System.Linq;
using CommandDotNet.Parsing;

namespace CommandDotNet.Directives
{
    public static class DirectivesTokenCollectionExtensions
    {
        public static bool TryGetDirective(this TokenCollection tokens, string directiveName, out string tokenValue)
        {
            tokenValue = tokens.Directives.FirstOrDefault(t =>
                    t.Value.Equals(directiveName, StringComparison.OrdinalIgnoreCase)
                    || t.Value.StartsWith($"{directiveName}:", StringComparison.OrdinalIgnoreCase))
                ?.Value;
            return tokenValue != null;
        }
    }
}