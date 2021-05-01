﻿using System;
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
            ParseCommand(commandContext, parseContext);
            if (parseContext.ParserError is { })
            {
                commandContext.ParseResult = new ParseResult(parseContext.ParserError);
            }
            return next(commandContext);
        }

        private static void ParseCommand(CommandContext commandContext, ParseContext parseContext)
        {
            foreach (var token in commandContext.Tokens.Arguments)
            {
                switch (token.TokenType)
                {
                    case TokenType.Argument:
                        if (parseContext.ExpectedOption is { })
                        {
                            ParseOptionValue(parseContext, token);
                        }
                        else
                        {
                            ParseValue(parseContext, token);
                        }
                        break;
                    case TokenType.Separator:
                        throw new ArgumentOutOfRangeException($"Bug: The argument separator should have already been processed and removed: {token.RawValue}");
                    case TokenType.Directive:
                        throw new ArgumentOutOfRangeException($"Bug: Directives should have already been processed and removed: {token.RawValue}");
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown {nameof(TokenType)}: {token.TokenType}");
                }

                if (parseContext.ParserError is { })
                {
                    return;
                }
            }

            if (parseContext.ExpectedOption is { }) // an option was left without a value
            {
                parseContext.ParserError = new MissingOptionValueParseError(parseContext.Command, parseContext.ExpectedOption);
                return;
            }

            if (commandContext.Tokens.Separated.Any() 
                && parseContext.Command.ArgumentSeparatorStrategy == ArgumentSeparatorStrategy.EndOfOptions)
            {
                commandContext.Tokens.Separated
                    .TakeWhile(t => parseContext.ParserError is null)
                    .ForEach(t => ParseValue(parseContext, t, operandsOnly:true));
            }

            commandContext.ParseResult = new ParseResult(
                parseContext.Command,
                parseContext.RemainingOperands.ToReadOnlyCollection(),
                commandContext.Tokens.Separated);
        }

        private static void ParseValue(ParseContext parseContext, Token token, bool operandsOnly = false)
        {
            if (parseContext.IgnoreRemainingArguments)
            {
                parseContext.RemainingOperands.Add(token);
            }
            else
            {
                if (!operandsOnly)
                {
                    if (TryParseSubcommand(parseContext, token))
                    {
                        return;
                    }
                    if (TryParseOption(parseContext, token))
                    {
                        return;
                    }
                }

                ParseOperand(parseContext, token);
            }
        }

        private static bool TryParseSubcommand(ParseContext parseContext, Token token)
        {
            if (parseContext.SubcommandsAreAllowed
                && parseContext.Command.FindArgumentNode(token.Value) is Command subcommand)
            {
                parseContext.Command = subcommand;
                return true;
            }

            return false;
        }

        private static bool TryParseOption(ParseContext parseContext, Token token)
        {
            if (!HasOptionPrefix(parseContext, token.RawValue, out var optionPrefix))
            {
                return false;
            }
            
            var optionName = token.RawValue.Substring(optionPrefix.Length);
            var assignmentIndex = optionName.IndexOfAny(new[] { ':', '=' });

            Token? optionToken, valueToken = null;
            if (assignmentIndex > 0)
            {
                var value = optionName.Substring(assignmentIndex + 1);
                optionName = optionName.Substring(0, assignmentIndex);

                valueToken = new Token(value, value, TokenType.Argument)
                {
                    SourceToken = token
                };
                optionToken = new Token($"{optionPrefix}{optionName}", optionName, TokenType.Argument)
                {
                    SourceToken = token
                };
            }
            else
            {
                optionToken = token;
            }

            var command = parseContext.Command;
            var node = parseContext.Command.FindArgumentNode(optionName);
            var option = node as Option;

            if (option is null)
            {
                if (node is Command cmd)
                {
                    var suggestion = parseContext.CommandContext.Original.Args.ToCsv(" ").Replace(token.RawValue, optionName);
                    parseContext.ParserError = new UnrecognizedArgumentParseError(parseContext.Command, token, optionPrefix, 
                        $"Unrecognized option '{token.RawValue}'. {Environment.NewLine}" +
                        $"If you intended to use the '{optionName}' command, try again with the following {Environment.NewLine}{Environment.NewLine}" +
                        suggestion);
                    return true;
                }
            }
            else 
            {
                if (!parseContext.ParseSettings.AllowSingleHyphenForLongNames
                    && optionPrefix == "-" && optionName.Length > 1)
                {
                    // must be long name, cannot be clubbed.
                    parseContext.ParserError = new UnrecognizedOptionParseError(command, optionToken, optionPrefix);
                    return true;
                }

                SetValue();
                return true;
            }

            if (optionPrefix == "--")
            {
                // must be long name, cannot be clubbed.
                parseContext.ParserError = new UnrecognizedOptionParseError(command, optionToken, optionPrefix);
                return true;
            }

            // TEST: invalid option where first characters could be short name but not all are
            if (optionPrefix == "-")
            {
                if (double.TryParse(optionName, out _))
                {
                    // is a negative number, 
                    return false;
                }
            }

            // is '-' or '/'.  both can be long or short named or clubbed/bundled flags
            // Long name and short name has already been checked above so this is either
            // a clubbed set of options or an invalid option name.
            // If it's an invalid option name, some of the letters could be match short names.
            //
            // * If all letters match short names, use them as short names
            // * If no letter match short name, raise error for the long name
            // * If some letters match short names, raise error for the long name only.
            //   Including short names will be confusing.
            //   The user should be able to determine the mistake.

            var clubbedOptions = optionName
                .ToCharArray()
                .Select(c => c.ToString())
                .Select((shortName, index) => (index,shortName,option:command.Find<Option>(shortName)))
                .TakeWhile(co => co.option is { })
                .ToList();

            if (clubbedOptions.Count < optionName.Length)
            {
                // TODO: Allow option value assignment without separator?
                //       This is common in some tools and supported by System.CommandLine
                parseContext.ParserError = new UnrecognizedOptionParseError(command, optionToken, optionPrefix);
                return false;
            }

            var origValueToken = valueToken;
            valueToken = null;
            foreach (var co in clubbedOptions)
            {
                var isLastOption = co.index == optionName.Length - 1;
                option = co.option;
                optionToken = new Token($"{optionPrefix}{co.shortName}", co.shortName, TokenType.Argument)
                {
                    SourceToken = token
                };

                if (isLastOption)
                {
                    if (origValueToken is { })
                    {
                        valueToken = origValueToken;
                    }
                }
                else if (!option!.IsFlag)
                {
                    // TEST: all options must be flags except last 
                    parseContext.ParserError = new ExpectedFlagParseError(command, token, co.shortName, co.option, 
                        $"'{co.shortName}' expects a value so it must be the last option specified in '{token.Value}'");
                    return true;
                }

                SetValue();
            }

            void SetValue()
            {
                if (option!.IsFlag)
                {
                    if (valueToken is null)
                    {
                        // only possible for flags which are boolean type
                        // Add a value to indicate that this option was specified
                        AddOptionValue(parseContext, option, optionToken, null, "true");
                    }
                    else
                    {
                        parseContext.ParserError = new UnexpectedOptionValueParseError(command, option, optionToken);
                    }
                }
                else
                {
                    parseContext.ExpectOption(option, optionToken);
                    if (valueToken is { })
                    {
                        ParseOptionValue(parseContext, valueToken);
                    }
                }
            }

            return true;
        }

        private static void ParseOperand(ParseContext parseContext, Token token)
        {
            if (parseContext.Operands.TryDequeue(out var operand))
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
                parseContext.ParserError = new UnrecognizedArgumentParseError(parseContext.Command, token, null,
                    $"Unrecognized command or argument '{token.RawValue}'");
            }
        }

        private static void ParseOptionValue(ParseContext parseContext, Token token)
        {
            var option = parseContext.ExpectedOption!;
            if (ValueIsAllowed(parseContext, option, token))
            {
                var optionToken = parseContext.ExpectedOptionToken!;
                var values = option.GetAlreadyParsedValues();

                if (option.Arity.AllowsMany())
                {
                    AddOptionValue(parseContext, option, optionToken, token, token.Value, values);
                }
                else if (option.IsFlag)
                {
                    throw new InvalidOperationException($"Bug: flag '{option.Name}' should have finished processing in {nameof(TryParseOption)}");
                }
                else if (values.Any())
                {
                    parseContext.ParserError = new UnexpectedOptionValueParseError(parseContext.Command, option, optionToken);
                }
                else
                {
                    AddOptionValue(parseContext, option, optionToken, token, token.Value, values);
                }
            }
        }

        private static bool HasOptionPrefix(ParseContext parseContext, string arg, out string optionPrefix)
        {
            // TODO: validate name is alphanumeric plus _ & - (where name is done)
            // TODO: return false if remaining is resulting name is not valid?
            //       - would force value to be treated as operand instead of throwing not-found option.

            if (arg.StartsWith("--") && !arg.StartsWith("---"))
            {
                optionPrefix = "--";
                return true;
            }

            // TODO: AppSettings to allow this prefix for long names?
            // TEST: - prefix for long names
            if (arg.StartsWith("-"))
            {
                optionPrefix = "-";
                return true;
            }

            // TODO: add AppHelpSettings{LongNamePrefix,ShortNamePrefix}
            //       Q: use callbacks so can be set based on OS and ENV?
            //       A: No, because it creates inconsistent user experience across machines
            // TODO: AppSettings to allow this prefix?
            // TEST: / prefix 
            if (parseContext.ParseSettings.AllowBackslashOptionPrefix && arg.StartsWith("/"))
            {
                optionPrefix = "/";
                return true;
            }

            optionPrefix = "";
            return false;
        }

        private static void AddOptionValue(ParseContext parseContext, Option option,
            Token optionToken, Token? valueToken, string? value = null, ICollection<ValueFromToken>? values = null)
        {
            values ??= option.GetAlreadyParsedValues();
            value ??= valueToken!.Value; //valueToken is not null when value is null

            values.Add(new ValueFromToken(value, valueToken, optionToken));
            parseContext.ClearOption();
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