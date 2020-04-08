using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    internal class CommandParser
    {
        private readonly AppSettings _appSettings;

        private CommandParser(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        internal static Task<int> ParseInputMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            try
            {
                new CommandParser(commandContext.AppConfig.AppSettings).ParseCommand(commandContext);
            }
            catch (CommandParsingException ex)
            {
                commandContext.ParseResult = new ParseResult(ex.Command, ex);
            }
            return next(commandContext);
        }

        private void ParseCommand(CommandContext commandContext)
        {
            bool ignoreRemainingArguments = false;
            var remainingOperands = new List<Token>();

            Command currentCommand = commandContext.RootCommand;
            Option currentOption = null;
            Token currentOptionToken = null;
            IEnumerator<Operand> operands = new OperandEnumerator(currentCommand.Operands);

            IEnumerable<Token> tokens = commandContext.Tokens.Arguments;

            // using TakeWhile ensures we're evaluating ParseSeparatedArguments
            // with the final command. We know this because separated arguments
            // can only contain operands.
            bool UsingEndOfOptions()
            {
                var separatorStrategy = currentCommand.GetArgumentSeparatorStrategy(_appSettings);
                return separatorStrategy == ArgumentSeparatorStrategy.EndOfOptions;
            }

            IEnumerable<Token> EndOfOptionsTokens()
            {
                if (!UsingEndOfOptions())
                {
                    yield break;
                }
                foreach (var token in commandContext.Tokens.Separated)
                {
                    yield return token;
                }
            }


            //tokens = tokens.Concat(EndOfOptionsTokens());

            tokens = tokens.Concat(commandContext.Tokens.Separated.TakeWhile(t => UsingEndOfOptions()));

            foreach (var token in tokens)
            {
                switch (token.TokenType)
                {
                    case TokenType.Option:
                        ParseOption(token, currentCommand, out currentOption);
                        if (currentOption != null)
                        {
                            currentOptionToken = token;
                        }
                        break;
                    case TokenType.Value:
                        if (ignoreRemainingArguments && currentOption == null)
                        {
                            remainingOperands.Add(token);
                        }
                        else
                        {
                            var operandResult = ParseArgumentValue(
                                token, ref currentCommand, ref currentOption, ref currentOptionToken, operands);

                            switch (operandResult)
                            {
                                case ParseOperandResult.Succeeded:
                                    break;
                                case ParseOperandResult.UnexpectedArgument:
                                    ignoreRemainingArguments = true;
                                    remainingOperands.Add(token);
                                    break;
                                case ParseOperandResult.NewSubCommand:
                                    operands = new OperandEnumerator(currentCommand.Operands);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(operandResult.ToString());
                            }
                        }
                        break;
                    case TokenType.Separator:
                        throw new ArgumentOutOfRangeException($"The argument list should have already had the separator removed: {token.RawValue}");
                    case TokenType.Directive:
                        throw new ArgumentOutOfRangeException($"Directives should have already been processed and removed: {token.RawValue}");
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown {nameof(TokenType)}: {token.TokenType}");
                }
            }

            if (currentOption != null) // an option was left without a value
            {
                throw new CommandParsingException(currentCommand, $"Missing value for option '{currentOption.Name}'");
            }

            commandContext.ParseResult = new ParseResult(
                currentCommand,
                remainingOperands,
                commandContext.Tokens.Separated);
        }

        private enum ParseOperandResult
        {
            Succeeded,
            UnexpectedArgument,
            NewSubCommand
        }

        private ParseOperandResult ParseArgumentValue(Token token,
            ref Command command, ref Option currentOption, ref Token currentOptionToken,
            IEnumerator<Operand> operands)
        {
            if (currentOption != null)
            {
                if (TryAddValue(currentOption, token, currentOptionToken))
                {
                    currentOption = null;
                    currentOptionToken = null;
                    return ParseOperandResult.Succeeded;
                }

                throw new CommandParsingException(command, $"Unexpected value '{token.RawValue}' for option '{currentOption.Name}'");
            }

            if (command.FindArgumentNode(token.Value) is Command subcommand)
            {
                command = subcommand;
                currentOption = null;
                currentOptionToken = null;
                return ParseOperandResult.NewSubCommand;
            }

            if (operands.MoveNext())
            {
                var current = operands.Current;
                GetArgumentParsedValues(current).Add(new ValueFromToken(token.Value, token, null));
            }
            else
            {
                if (command.GetIgnoreUnexpectedOperands(_appSettings))
                {
                    return ParseOperandResult.UnexpectedArgument;
                }

                // use the term "argument" for messages displayed to users
                throw new CommandParsingException(command, $"Unrecognized command or argument '{token.RawValue}'", unrecognizedArgument: token);
            }

            return ParseOperandResult.Succeeded;
        }

        private void ParseOption(Token token, Command command, out Option option)
        {
            var optionTokenType = token.OptionTokenType;

            option = command.FindOption(optionTokenType.GetName());
            if (option == null)
            {
                throw new CommandParsingException(command, $"Unrecognized option '{token.RawValue}'", unrecognizedArgument: token);
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
                TryAddValue(option, null, token);
                option = null;
            }
        }

        private static ICollection<ValueFromToken> GetArgumentParsedValues(IArgument argument)
        {
            // in most cases, this will be the first or only InputValues
            var source = Constants.InputValueSources.Argument;
            var parserValues = argument.InputValues.FirstOrDefault(iv => iv.Source == source);
            if (parserValues == null)
            {
                parserValues = new InputValue(source, false, new List<ValueFromToken>());
                argument.InputValues.Add(parserValues);
            }

            return (List<ValueFromToken>)parserValues.ValuesFromTokens;
        }

        private static bool TryAddValue(Option option, Token valueToken, Token optionToken)
        {
            var values = GetArgumentParsedValues(option);

            if (option.Arity.AllowsMany())
            {
                values.Add(new ValueFromToken(valueToken.Value, valueToken, optionToken));
            }
            else if (option.Arity.AllowsNone())
            {
                // Add a value to indicate that this option was specified
                values.Add(new ValueFromToken("true", valueToken, optionToken));
            }
            else if (!option.Arity.AllowsMany())
            {
                if (values.Any())
                {
                    return false;
                }
                values.Add(new ValueFromToken(valueToken.Value, valueToken, optionToken));
            }
            return true;
        }

        private class OperandEnumerator : IEnumerator<Operand>
        {
            private readonly IEnumerator<Operand> _enumerator;

            public OperandEnumerator(IEnumerable<Operand> enumerable)
            {
                _enumerator = enumerable.GetEnumerator();
            }

            public Operand Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (Current == null || !Current.Arity.AllowsMany())
                {
                    return _enumerator.MoveNext();
                }

                // If current operand allows multiple values, we don't move forward and
                // all later values will be added to current IOperand.Values
                return true;
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }
    }
}