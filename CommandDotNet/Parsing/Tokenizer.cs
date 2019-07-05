using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Parsing
{
    public static class Tokenizer
    {
        public static TokenCollection Tokenize(this IEnumerable<string> args, bool includeDirectives = false)
        {
            return new TokenCollection(ParseTokens(args, includeDirectives));
        }

        public static Token Tokenize(string arg, bool includeDirectives = false)
        {
            Token token;
            return (includeDirectives && TryTokenizeDirective(arg, out token))
                   || TryTokenizeSeparator(arg, out token)
                   || TryTokenizeOption(arg, out token)
                ? token
                : TokenizeValue(arg);
        }

        public static bool TryTokenizeDirective(string arg, out Token token)
        {
            if (arg.Length > 2 && arg[0] == '[' && arg[arg.Length - 1] == ']')
            {
                token = new Token(arg, arg.Substring(1, arg.Length - 2), TokenType.Directive);
                return true;
            }

            token = null;
            return false;
        }

        public static bool TryTokenizeSeparator(string arg, out Token token)
        {
            if (arg == "--")
            {
                token = new Token(arg, arg, TokenType.Separator);
                return true;
            }

            token = null;
            return false;
        }

        public static bool TryTokenizeOption(string arg, out Token token)
        {
            if (arg.Length <= 1 || arg[0] != '-' || arg.StartsWith("---"))
            {
                token = null;
                return false;
            }

            bool isLongOption = arg[1] == '-';
            bool isShortOption = !isLongOption;

            var value = arg.Substring(isShortOption ? 1 : 2);
            var assignmentIndex = value.IndexOfAny(new[] { ':', '=' });
            var hasValue = assignmentIndex > 0;
            var isClubbed = isShortOption && !hasValue && value.Length > 1;

            token = new Token(arg, arg.Substring(isShortOption ? 1 : 2), TokenType.Option,
                new OptionTokenType(value, isLongOption, isShortOption, isClubbed, hasValue, assignmentIndex));
            return true;
        }

        public static Token TokenizeValue(string arg)
        {
            return new Token(arg, arg, TokenType.Value);
        }

        public static TokenCollection ExpandClubbedOptions(this TokenCollection tokenCollection)
        {
            return new TokenCollection(tokenCollection.SelectMany(ExpandClubbedOption));
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

        private static IEnumerable<Token> ParseTokens(IEnumerable<string> args, bool includeDirectives)
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