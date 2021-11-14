using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Tokens
{
    public static class Tokenizer
    {
        internal static Token SeparatorToken { get; } = new("--", "--", TokenType.Separator);

        public static TokenCollection Tokenize(this IEnumerable<string> args, bool includeDirectives = false, string sourceName = "args")
        {
            return new TokenCollection(ParseTokens(args, includeDirectives, sourceName));
        }

        public static Token Tokenize(string arg, bool includeDirectives = false, string sourceName = "args")
        {
            Token token = (includeDirectives && TryTokenizeDirective(arg, out Token? parsedToken))
                                || TryTokenizeSeparator(arg, out parsedToken)
                ? parsedToken!
                : TokenizeValue(arg);
            token.SourceName = sourceName;
            return token;
        }

        public static bool TryTokenizeDirective(string arg, out Token? token)
        {
            if (arg.Length > 2 && arg[0] == '[' && arg[^1] == ']')
            {
                token = new Token(arg, arg.Substring(1, arg.Length - 2), TokenType.Directive);
                return true;
            }

            token = null;
            return false;
        }

        public static bool TryTokenizeSeparator(string arg, out Token? token)
        {
            if (arg == SeparatorToken.Value)
            {
                token = new Token(arg, arg, TokenType.Separator);
                return true;
            }

            token = null;
            return false;
        }

        public static Token TokenizeValue(string argValue)
        {
            return new Token(argValue, argValue, TokenType.Argument);
        }

        public static string[] ToArgsArray(this IEnumerable<Token> tokens)
        {
            return tokens.Select(t => t.RawValue).ToArray();
        }

        private static IEnumerable<Token> ParseTokens(IEnumerable<string> args, bool includeDirectives, string sourceName)
        {
            bool foundSeparator = false;

            foreach (var arg in args)
            {
                if (foundSeparator)
                {
                    yield return new Token(arg, arg, TokenType.Argument){SourceName = sourceName};
                }
                else
                {
                    var token = Tokenize(arg, includeDirectives, sourceName);

                    includeDirectives = includeDirectives && token.TokenType == TokenType.Directive;
                    foundSeparator = token.TokenType == TokenType.Separator;

                    yield return token;
                }
            }
        }
    }
}