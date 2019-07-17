using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

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

            token = new Token(arg, value, TokenType.Option,
                new OptionTokenType(value, isLongOption, isClubbed, hasValue, assignmentIndex));
            return true;
        }

        public static Token TokenizeValue(string arg)
        {
            return new Token(arg, arg, TokenType.Value);
        }

        public static string[] ToArgsArray(this TokenCollection tokens)
        {
            var results = tokens.Directives.Select(t => t.RawValue)
                .Union(tokens.Arguments.Select(t => t.RawValue));
            if (tokens.Separated.Any())
            {
                results = results.Union("--".ToEnumerable())
                    .Union(tokens.Separated.Select(t => t.RawValue));
            }
            return results.ToArray();
        }

        public static string[] ToArgsArray(this IEnumerable<Token> tokens)
        {
            return tokens.Select(t => t.RawValue).ToArray();
        }

        public static TokenCollection ExpandClubbedOptions(this TokenCollection tokenCollection)
        {
            return new TokenCollection(tokenCollection.SelectMany(ExpandClubbedOption));
        }

        public static TokenCollection SplitOptionAssignments(this TokenCollection tokenCollection)
        {
            return new TokenCollection(tokenCollection.SelectMany(SplitOptionAssignment));
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

        private static IEnumerable<Token> ExpandClubbedOption(Token token)
        {
            if (token.TokenType == TokenType.Option && token.OptionTokenType.IsClubbed)
            {
                foreach (var flag in token.Value.ToCharArray())
                {
                    var value = flag.ToString();
                    yield return new Token($"-{value}", value, TokenType.Option, new OptionTokenType(value));
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