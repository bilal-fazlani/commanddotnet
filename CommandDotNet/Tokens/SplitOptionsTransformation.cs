using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Parsing;

namespace CommandDotNet.Tokens
{
    internal static class SplitOptionsTransformation
    {
        internal static TokenCollection Transform(this TokenCollection tokenCollection)
        {
            return tokenCollection.Transform(SplitOptionAssignment, skipDirectives: true, skipSeparated: true);
        }

        private static IEnumerable<Token> SplitOptionAssignment(Token token)
        {
            if (token.TokenType == TokenType.Option && token.OptionTokenType.HasValue)
            {
                var prefix = token.OptionTokenType.GetPrefix();
                var optionName = token.OptionTokenType.GetName();
                var value = token.OptionTokenType.GetAssignedValue();

                yield return new Token(
                    $"{prefix}{optionName}", optionName, TokenType.Option,
                    new OptionTokenType(optionName, token.OptionTokenType.IsLong));
                yield return new Token(value, value, TokenType.Value);
            }
            else
            {
                yield return token;
            }
        }
    }
}