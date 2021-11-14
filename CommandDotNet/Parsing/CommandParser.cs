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
                        throw new ArgumentOutOfRangeException($"Bug: The argument separator should have already been processed and removed: {token.RawValue}");
                    case TokenType.Directive:
                        throw new ArgumentOutOfRangeException($"Bug: Directives should have already been processed and removed: {token.RawValue}");
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown {nameof(TokenType)}: {token.TokenType}");
                }
            }

            if (parseContext.ParserError is { })
            {
                return;
            }

            if (parseContext.Option is { }) // an option was left without a value
            {
                parseContext.ParserError = new MissingOptionValueParseError(parseContext.Command, parseContext.Option);
                return;
            }

            if (commandContext.Tokens.Separated.Any() 
                && parseContext.Command.ArgumentSeparatorStrategy == ArgumentSeparatorStrategy.EndOfOptions)
            {
                commandContext.Tokens.Separated
                    .TakeWhile(t => parseContext.ParserError is null)
                    .ForEach(t => ParseNonOptionValue(parseContext, t));

                // if (parseContext.ParserError is { })
                // {
                //     return;
                // }
            }

            commandContext.ParseResult = new ParseResult(
                parseContext.Command,
                parseContext.RemainingOperands.ToReadOnlyCollection(),
                commandContext.Tokens.Separated);
        }

        private static void ParseOption(ParseContext parseContext, Token token)
        {
            var optionTokenType = token.OptionTokenType!;

            var node = parseContext.Command.FindArgumentNode(optionTokenType.GetName());
            var option = node as Option;
            if (option is null)
            {
                if (node is {} && node is Command cmd)
                {
                    var suggestion = parseContext.CommandContext.Original.Args.ToCsv(" ").Replace(token.RawValue, optionTokenType.GetName());
                    parseContext.ParserError = new UnrecognizedArgumentParseError(parseContext.Command, token, $"Unrecognized option '{token.RawValue}'. {Environment.NewLine}" +
                        $"If you intended to use the '{optionTokenType.GetName()}' command, try again with the following {Environment.NewLine}{Environment.NewLine}" +
                        suggestion);
                }
                else
                {
                    parseContext.ParserError = new UnrecognizedArgumentParseError(parseContext.Command, token, $"Unrecognized option '{token.RawValue}'");
                }
                return;
            }

            if (optionTokenType.IsClubbed)
            {
                throw new InvalidOperationException($"Bug: ExpandClubbedOptions transformation should have expanded all clubbed tokens: {token}");
            }
            if (optionTokenType.HasValue)
            {
                throw new InvalidOperationException($"Bug: SplitOptionAssignments transformation should have split values from all option tokens: {token}");
            }

            if (option.Arity.RequiresNone())
            {
                var values = option.GetAlreadyParsedValues();
                // only possible for flags which are boolean type
                // Add a value to indicate that this option was specified
                AddOptionValue(parseContext, option, values, token, null, "true");
            }
            else
            {
                parseContext.ExpectOption(option, token);
            }
        }

        private static void ParseOptionValue(ParseContext parseContext, Token token)
        {
            var option = parseContext.Option!;
            if (ValueIsAllowed(parseContext, option, token))
            {
                var optionToken = parseContext.OptionToken!;
                var values = option.GetAlreadyParsedValues();

                if (option.Arity.AllowsMany())
                {
                    AddOptionValue(parseContext, option, values, optionToken, token, token.Value);
                }
                else if (option.Arity.RequiresNone())
                {
                    throw new InvalidOperationException($"Bug: flag '{option.Name}' should have finished processing in {nameof(ParseOption)}");
                }
                else if (values.Any())
                {
                    parseContext.ParserError = new UnexpectedOptionValueParseError(parseContext.Command, option, optionToken);
                }
                else
                {
                    AddOptionValue(parseContext, option, values, optionToken, token, token.Value);
                }

            }
        }

        private static void AddOptionValue(ParseContext parseContext, Option option, ICollection<ValueFromToken> values,
            Token optionToken, Token? valueToken, string value)
        {
            values.Add(new ValueFromToken(value, valueToken, optionToken));
            parseContext.ClearOption();
            if (!option.IsInterceptorOption || option.AssignToExecutableSubcommands)
            {
                parseContext.CommandArgumentParsed();
            }
        }

        private static void ParseNonOptionValue(ParseContext parseContext, Token token)
        {
            if (parseContext.IgnoreRemainingArguments)
            {
                parseContext.RemainingOperands.Add(token);
            }
            else if (parseContext.SubcommandsAreAllowed
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
                    parseContext.CommandArgumentParsed();
                }
            }
            else if (parseContext.Command.IgnoreUnexpectedOperands)
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

        private static ICollection<ValueFromToken> GetAlreadyParsedValues(this IArgument argument)
        {
            // in most cases, this will be the first and only InputValues
            var source = (string)Resources.A.Common_argument_lc;
            var parserValues = argument.InputValues.FirstOrDefault(iv => iv.Source == source);

            if (parserValues is null)
            {
                parserValues = new InputValue(source, false, new List<ValueFromToken>());
                argument.InputValues.Add(parserValues);
            }
            parserValues.ValuesFromTokens ??= new List<ValueFromToken>();

            return (List<ValueFromToken>)parserValues.ValuesFromTokens!;
        }
    }
}