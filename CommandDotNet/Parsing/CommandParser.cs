using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.ClassModeling;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    internal class CommandParser
    {
        private readonly AppSettings _appSettings;

        public CommandParser(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public void ParseCommand(ExecutionContext executionContext, ICommand command)
        {
            bool ignoreRemainingArguments = false;
            var remainingArguments = new List<string>();

            ICommand currentCommand = command;
            IOption currentOption = null;
            IEnumerator<IOperand> operands = new OperandEnumerator(command.Operands);

            var argumentValues = new Dictionary<string, ArgumentValues>();
            List<string> GetArgValue(IArgument argument)
            {
                var argValue = argumentValues.GetOrAdd(argument.Aliases.First(), k =>
                {
                    // link values w/ ValueInfo until we can remove it completely
                    var argumentInfo = argument.ContextData.Get<ArgumentInfo>();
                    var values = argumentInfo != null ? argumentInfo.ValueInfo.Values : new List<string>();
                    return new ArgumentValues(argument, values);
                });
                return argValue.Values;
            }

            foreach (var token in executionContext.Tokens.Arguments)
            {
                switch (token.TokenType)
                {
                    case TokenType.Option:
                        var optionResult = ParseOption(
                            token, currentCommand, out currentOption, GetArgValue);

                        switch (optionResult)
                        {
                            case ParseOptionResult.Succeeded:
                                if (currentOption?.InvokeAsCommand != null)
                                {
                                    currentOption.InvokeAsCommand();
                                    executionContext.ParseResult = new ParseResult(
                                        currentCommand, 
                                        null, 
                                        argumentValues.Values.ToList().AsReadOnly());
                                    executionContext.ShouldExitWithCode(0);
                                    return;
                                }
                                break;
                            case ParseOptionResult.UnknownOption:
                                remainingArguments.Add(token.Value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(optionResult.ToString());
                        }
                        break;
                    case TokenType.Value:
                        if (ignoreRemainingArguments && currentOption == null)
                        {
                            remainingArguments.Add(token.Value);
                        }
                        else
                        {
                            var operandResult = ParseArgumentValue(
                                token, ref currentCommand, ref currentOption, operands, GetArgValue);

                            switch (operandResult)
                            {
                                case ParseOperandResult.Succeeded:
                                    break;
                                case ParseOperandResult.UnexpectedArgument:
                                    ignoreRemainingArguments = true;
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

            remainingArguments.AddRange(executionContext.Tokens.Separated.Select(t => t.RawValue));

            if (currentOption != null) // an option was left without a value
            {
                throw new CommandParsingException(currentCommand, $"Missing value for option '{currentOption.Name}'");
            }
            executionContext.ParseResult = new ParseResult(
                currentCommand, 
                remainingArguments.AsReadOnly(), 
                argumentValues.Values.ToList().AsReadOnly());
        }

        private enum ParseOperandResult
        {
            Succeeded,
            UnexpectedArgument,
            NewSubCommand
        }

        private enum ParseOptionResult
        {
            Succeeded,
            UnknownOption
        }

        private ParseOperandResult ParseArgumentValue(Token token,
            ref ICommand command,
            ref IOption option,
            IEnumerator<IOperand> operands, 
            Func<IArgument,List<string>> getArgValues)
        {
            if (option != null)
            {
                if (TryAddValue(option, token.Value, getArgValues))
                {
                    option = null;
                    return ParseOperandResult.Succeeded;
                }

                throw new CommandParsingException(command, $"Unexpected value '{token.RawValue}' for option '{option.Name}'");
            }

            var subCommand = command.Commands
                .FirstOrDefault(c => c.Name.Equals(token.Value, StringComparison.OrdinalIgnoreCase));
            if (subCommand != null)
            {
                command = subCommand;
                option = null;
                return ParseOperandResult.NewSubCommand;
            }

            if (operands.MoveNext())
            {
                getArgValues(operands.Current).Add(token.Value);
            }
            else
            {
                if (_appSettings.ThrowOnUnexpectedArgument)
                {
                    // use the term "argument" for messages displayed to users
                    throw new CommandParsingException(command, $"Unrecognized command or argument '{token.RawValue}'");
                }
                return ParseOperandResult.UnexpectedArgument;
            }

            return ParseOperandResult.Succeeded;
        }

        private ParseOptionResult ParseOption(Token token, 
            ICommand command, 
            out IOption option,
            Func<IArgument, List<string>> getArgValues)
        {
            var optionTokenType = token.OptionTokenType;

            string optionName = optionTokenType.GetName();

            // TODO: use IOption for param
            option = command.FindOption(optionName);

            if (option == null)
            {
                if (_appSettings.ThrowOnUnexpectedArgument)
                {
                    throw new CommandParsingException(command, $"Unrecognized option '{token.RawValue}'");
                }
                return ParseOptionResult.UnknownOption;
            }

            if (option.IsSystemOption)
            {
                return ParseOptionResult.Succeeded;
            }
            if (optionTokenType.IsClubbed)
            {
                throw new AppRunnerException($"ExpandClubbedOptions transformation should have expanded all clubbed tokens: {token}");
            }
            if (optionTokenType.HasValue)
            {
                throw new AppRunnerException($"SplitOptionAssignments transformation should have split values from all option tokens: {token}");
            }
            if(option.Arity.AllowsNone())
            {
                // No value is needed for this option
                TryAddValue(option, null, getArgValues);
                option = null;
            }

            return ParseOptionResult.Succeeded;
        }
        private static bool TryAddValue(IOption option, string value, Func<IArgument, List<string>> getArgValues)
        {
            if (option.Arity.AllowsZeroOrMore())
            {
                getArgValues(option).Add(value);
            }
            else if (option.Arity.AllowsZeroOrOne())
            {
                if (getArgValues(option).Any())
                {
                    return false;
                }
                getArgValues(option).Add(value);
            }
            else if (option.Arity.AllowsNone())
            {
                if (value != null)
                {
                    return false;
                }
                // Add a value to indicate that this option was specified
                getArgValues(option).Add("true");
            }
            return true;
        }

        private class OperandEnumerator : IEnumerator<IOperand>
        {
            private readonly IEnumerator<IOperand> _enumerator;

            public OperandEnumerator(IEnumerable<IOperand> enumerable)
            {
                _enumerator = enumerable.GetEnumerator();
            }

            public IOperand Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (Current == null || !Current.Arity.AllowsZeroOrMore())
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