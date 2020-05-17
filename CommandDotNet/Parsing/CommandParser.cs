using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    internal static partial class CommandParser
    {
        internal static Task<int> ParseInputMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            var parseContext = new ParseContext(commandContext, new Queue<Token>(commandContext.Tokens.Arguments));
            CommandParser.ParseCommand(commandContext, parseContext);
            if (parseContext.ParserError is { })
            {
                commandContext.ParseResult = new ParseResult(parseContext.ParserError);
            }
            return next(commandContext);
        }

        private static void ParseCommand(CommandContext commandContext, ParseContext parseContext)
        {
            foreach (var token in commandContext.Tokens.Arguments.TakeWhile(t => parseContext.ParserError is null))
            {
                switch (token.TokenType)
                {
                    case TokenType.Option:
                        if (parseContext.Option is { })
                        {
                            parseContext.ParserError = new MissingOptionValueParseError(parseContext.Command, parseContext.Option);
                            return;
                        }
                        ParseOption(parseContext, token);
                        break;
                    case TokenType.Value:
                        if (parseContext.Option is { })
                        {
                            ParseOptionValue(parseContext, token);
                        }
                        else
                        {
                            ParseNonOptionValue(parseContext, token);
                        }
                        break;
                    case TokenType.Separator:
                        throw new ArgumentOutOfRangeException($"The argument separator should have already been processed and removed: {token.RawValue}");
                    case TokenType.Directive:
                        throw new ArgumentOutOfRangeException($"Directives should have already been processed and removed: {token.RawValue}");
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown {nameof(TokenType)}: {token.TokenType}");
                }
            }

            if (parseContext.ParserError is { })
            {
                // TEST: too many EndOfOptions arguments
                //       current test failure is a side-effect.
                //       make explicit test case.
                return;
            }

            if (parseContext.Option is { }) // an option was left without a value
            {
                // TEST: option left without a value at end of input
                parseContext.ParserError = new MissingOptionValueParseError(parseContext.Command, parseContext.Option);
                return;
            }

            if (commandContext.Tokens.Separated.Any() && parseContext.UsingEndOfOptions())
            {
                commandContext.Tokens.Separated
                    .TakeWhile(t => parseContext.ParserError is null)
                    .ForEach(t => ParseNonOptionValue(parseContext, t));

                // TEST: too many operands after EndOfOptions when IgnoreUnexpectedOperands is false
                if (parseContext.ParserError is { })
                {
                    return;
                }
            }

            commandContext.ParseResult = new ParseResult(
                parseContext.Command,
                parseContext.RemainingOperands.ToReadOnlyCollection(),
                commandContext.Tokens.Separated);
        }

        private static void ParseOptionValue(ParseContext parseContext, Token token)
        {
            var option = parseContext.Option!;
            if (ValueIsAllowed(parseContext, option, token))
            {
                var optionToken = parseContext.OptionToken!;
                if (option.TryAddValue(token, optionToken))
                {
                    parseContext.ClearOption();
                }
                else
                {
                    parseContext.ParserError = new UnexpectedOptionValueParseError(parseContext.Command, option, optionToken);
                }
            }
        }

        private static void ParseNonOptionValue(ParseContext parseContext, Token token)
        {
            if (parseContext.IgnoreRemainingArguments)
            {
                parseContext.RemainingOperands.Add(token);
            }
            else if (!parseContext.Operands.HasSuppliedOperands
                     && parseContext.Command.FindArgumentNode(token.Value) is Command subcommand)
            {
                parseContext.Command = subcommand;
            }
            else if (parseContext.Operands.TryDequeue(out var operand))
            {
                var currentOperand = operand!;
                if (ValueIsAllowed(parseContext, currentOperand, token))
                {
                    currentOperand
                        .GetAlreadyParsedValues()
                        .Add(new ValueFromToken(token.Value, token, null));
                }
            }
            else if (parseContext.Command.GetIgnoreUnexpectedOperands(parseContext.AppSettings))
            {
                parseContext.IgnoreRemainingArguments = true;
                parseContext.RemainingOperands.Add(token);
            }
            else
            {
                // use the term "argument" for messages displayed to users
                parseContext.ParserError = new UnrecognizedArgumentParseError(parseContext.Command, token, $"Unrecognized command or argument '{token.RawValue}'");
            }
        }

        private static void ParseOption(ParseContext parseContext, Token token)
        {
            var optionTokenType = token.OptionTokenType!;

            var option = parseContext.Command.Find<Option>(optionTokenType.GetName());
            if (option is null)
            {
                parseContext.ParserError = new UnrecognizedArgumentParseError(parseContext.Command, token, $"Unrecognized option '{token.RawValue}'");
                return;
            }

            if (optionTokenType.IsClubbed)
            {
                throw new AppRunnerException($"ExpandClubbedOptions transformation should have expanded all clubbed tokens: {token}");
            }
            if (optionTokenType.HasValue)
            {
                throw new AppRunnerException($"SplitOptionAssignments transformation should have split values from all option tokens: {token}");
            }
            if (option.Arity.AllowsNone())
            {
                option.TryAddValue(null, token);
                parseContext.ClearOption();
                return;
            }

            parseContext.ExpectOption(option, token);
        }

        private static bool ValueIsAllowed(ParseContext parseContext, IArgument argument, Token token)
        {
            if (argument.AllowedValues.IsNullOrEmpty() || argument.AllowedValues.Contains(token.Value))
            {
                return true;
            }

            // help displays the AllowedValues
            // TypoSuggestions can hide the help
            parseContext.CommandContext.ShowHelpOnExit = true;
            parseContext.ParserError = new NotAllowedValueParseError(parseContext.Command, argument, token);
            return false;
        }

        private static bool TryAddValue(this Option option, Token? valueToken, Token optionToken)
        {
            var values = option.GetAlreadyParsedValues();

            if (option.Arity.AllowsMany())
            {
                values.Add(new ValueFromToken(valueToken!.Value, valueToken, optionToken));
            }
            else if (option.Arity.AllowsNone())
            {
                // only possible for flags which are boolean type
                // Add a value to indicate that this option was specified
                values.Add(new ValueFromToken("true", valueToken, optionToken));
            }
            else
            {
                if (values.Any())
                {
                    return false;
                }
                values.Add(new ValueFromToken(valueToken!.Value, valueToken, optionToken));
            }
            return true;
        }

        private static ICollection<ValueFromToken> GetAlreadyParsedValues(this IArgument argument)
        {
            // in most cases, this will be the first or only InputValues
            var source = Constants.InputValueSources.Argument;
            var parserValues = argument.InputValues.FirstOrDefault(iv => iv.Source == source);

            if (parserValues is null)
            {
                parserValues = new InputValue(source, false, new List<ValueFromToken>());
                argument.InputValues.Add(parserValues);
            }
            parserValues.ValuesFromTokens ??= new List<ValueFromToken>();

            return (List<ValueFromToken>)parserValues.ValuesFromTokens!;
        }

        private static bool UsingEndOfOptions(this ParseContext parseContext)
        {
            var separatorStrategy = parseContext.Command.GetArgumentSeparatorStrategy(parseContext.AppSettings);
            return separatorStrategy == ArgumentSeparatorStrategy.EndOfOptions;
        }
    }
}