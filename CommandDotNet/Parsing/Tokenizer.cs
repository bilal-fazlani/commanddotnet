using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Parsing
{
    public static class Tokenizer
    {
        public static Tokens Tokenize(this string[] args, bool includeDirectives = false)
        {
            return new Tokens(ParseTokens(args, includeDirectives));
        }

        public static Token Tokenize(this string arg, bool includeDirectives = false)
        {
            if (includeDirectives && arg.Length > 2 && arg[0] == '[' && arg[arg.Length - 1] == ']')
            {
                return new Token(arg, arg.Substring(1, arg.Length - 2), TokenType.Directive);
            }

            if (arg.Length > 1 && arg[0] == '-' && !arg.StartsWith("---"))
            {
                bool isLongOption = arg[1] == '-';
                bool isShortOption = !isLongOption;

                if (isLongOption && arg == "--")
                {
                    return new Token(arg, arg, TokenType.Separator);
                }

                var value = arg.Substring(isShortOption ? 1 : 2);
                var assignmentIndex = value.IndexOfAny(new[] { ':', '=' });
                var hasValue = assignmentIndex > 0;
                var isClubbed = isShortOption && !hasValue && value.Length > 1;

                return new Token(arg, arg.Substring(isShortOption ? 1 : 2), TokenType.Option,
                    new OptionTokenType(value, isLongOption, isShortOption, isClubbed, hasValue, assignmentIndex));
            }

            return new Token(arg, arg, TokenType.Value);
        }

        public static Tokens ExpandClubbedOptions(this Tokens tokens)
        {
            return new Tokens(tokens.SelectMany(ExpandClubbedOption));
        }

        private static IEnumerable<Token> ExpandClubbedOption(Token token)
        {
            if (token.TokenType == TokenType.Option && token.OptionTokenType.IsClubbed)
            {
                foreach (var flag in token.Value.ToCharArray())
                {
                    var value = flag.ToString();
                    yield return new Token($"-{value}", value, TokenType.Option, new OptionTokenType(value, isShort: true));
                }
            }
            else
            {
                yield return token;
            }
        }

        private static IEnumerable<Token> ParseTokens(string[] args, bool includeDirectives)
        {
            bool foundSeparator = false;

            foreach (var arg in args)
            {
                if (foundSeparator)
                {
                    yield return new Token(arg, arg, TokenType.Value);
                }

                var token = Tokenize(arg, includeDirectives);

                includeDirectives = includeDirectives && token.TokenType == TokenType.Directive;
                foundSeparator = token.TokenType == TokenType.Separator;

                yield return token;
            }
        }
    }
}