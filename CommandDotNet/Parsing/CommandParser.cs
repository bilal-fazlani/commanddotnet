using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Exceptions;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet.Parsing
{
    internal class CommandParser
    {
        private readonly AppSettings _appSettings;
        private readonly ParserContext _parserContext;

        public CommandParser(AppSettings appSettings, ParserContext parserContext)
        {
            _appSettings = appSettings;
            _parserContext = parserContext;
        }

        public ParseResult ParseCommand(CommandLineApplication app, string[] args)
        {
            ICommand currentCommand = app;
            IOption currentOption = null;
            IEnumerator<IOperand> arguments = new OperandEnumerator(app.Operands);

            var remainingArguments = new List<Token>();

            var tokens = args.Tokenize(includeDirectives: _appSettings.EnableDirectives);

            tokens = ApplyInputTransformations(tokens);

            if (_parserContext.ParseDirectiveEnabled)
            {
                return new ParseResult(app, args, tokens, exitCode: 0);
            }

            bool ignoreRemainingArguments = false;

            foreach (var token in tokens)
            {
                if (ignoreRemainingArguments)
                {
                    remainingArguments.Add(token);
                    continue;
                }

                switch (token.TokenType)
                {
                    case TokenType.Option:
                        var optionResult = ParseOption(token, currentCommand, out currentOption);
                        switch (optionResult)
                        {
                            case ParseOptionResult.Succeeded:
                                break;
                            case ParseOptionResult.UnexpectedArgument:
                                ignoreRemainingArguments = true;
                                break;
                            case ParseOptionResult.ShowHelp:
                                HelpService.Print(_appSettings, currentCommand);
                                return new ParseResult(currentCommand, args, tokens, exitCode: 0);
                            case ParseOptionResult.ShowVersion:
                                VersionService.Print(_appSettings);
                                return new ParseResult(currentCommand, args, tokens, exitCode: 0);
                            default:
                                throw new ArgumentOutOfRangeException(optionResult.ToString());
                        }
                        break;
                    case TokenType.Value:
                        var operandResult = ParseArgumentValue(token, ref currentCommand, ref currentOption, arguments);
                        switch (operandResult)
                        {
                            case ParseOperandResult.Succeeded:
                                break;
                            case ParseOperandResult.UnexpectedArgument:
                                ignoreRemainingArguments = true;
                                break;
                            case ParseOperandResult.NewSubCommand:
                                arguments = new OperandEnumerator(currentCommand.Operands);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(operandResult.ToString());
                        }
                        break;
                    case TokenType.Separator:
                        ignoreRemainingArguments = true;
                        break;
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

            return new ParseResult(currentCommand, args, tokens, unparsedTokens: new Tokens(remainingArguments));
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
            UnexpectedArgument,
            ShowHelp,
            ShowVersion,
        }

        private ParseOperandResult ParseArgumentValue(
            Token token, 
            ref ICommand command,
            ref IOption option, 
            IEnumerator<IOperand> operands)
        {
            if (option != null)
            {
                if (TryAddValue(option, token.Value))
                {
                    option = null;
                    return ParseOperandResult.Succeeded;
                }

                throw new CommandParsingException(command, $"Unexpected value '{token.Value}' for option '{option.Name}'");
            }

            var subCommand = command.Commands
                .Where(c => c.Name.Equals(token.Value, StringComparison.OrdinalIgnoreCase))
                .Cast<CommandLineApplication>()
                .FirstOrDefault();
            if (subCommand != null)
            {
                command = subCommand;
                option = null;
                return ParseOperandResult.NewSubCommand;
            }

            if (operands.MoveNext())
            {
                operands.Current.Values.Add(token.Value);
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

        private ParseOptionResult ParseOption(Token token, ICommand command, out IOption option)
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
                return ParseOptionResult.UnexpectedArgument;
            }

            if (option.Name == Constants.HelpArgumentTemplate.Name)
            {
                return ParseOptionResult.ShowHelp;
            }

            if (option.Name == Constants.VersionArgumentTemplate.Name)
            {
                return ParseOptionResult.ShowVersion;
            }

            if (optionTokenType.HasValue)
            {
                if (!TryAddValue(option, optionTokenType.GetAssignedValue()))
                {
                    throw new CommandParsingException(command, $"Unexpected value '{token.Value}' for option '{option.Name}'");
                }

                option = null;
            }
            else if(option.Arity.AllowsNone())
            {
                // No value is needed for this option
                TryAddValue(option, null);
                option = null;
            }

            return ParseOptionResult.Succeeded;
        }
        private static bool TryAddValue(IOption option, string value)
        {
            if (option.Arity.AllowsZeroOrMore())
            {
                option.Values.Add(value);
            }
            else if (option.Arity.AllowsZeroOrOne())
            {
                if (option.Values.Any())
                {
                    return false;
                }
                option.Values.Add(value);
            }
            else if (option.Arity.AllowsNone())
            {
                if (value != null)
                {
                    return false;
                }
                // Add a value to indicate that this option was specified
                option.Values.Add("true");
            }
            return true;
        }
        private Tokens ApplyInputTransformations(Tokens args)
        {
            if (_parserContext.ParseDirectiveEnabled)
            {
                ReportTransformation(null, args);
            }

            var transformations = _parserContext.InputTransformations.OrderBy(t => t.Order).AsEnumerable();

            // append ExpandClubbedFlags to the end.
            // it's a feature we want to ensure is applied to all arguments
            // to prevent cases later where short clubbed options aren't found
            transformations = transformations.Union(
                new[]
            {
                new InputTransformation(
                    "Expand clubbed flags",
                    int.MaxValue,
                    Tokenizer.ExpandClubbedOptions),
            });

            foreach (var transformation in transformations)
            {
                try
                {
                    var tempArgs = transformation.Transformation(args);

                    if (_parserContext.ParseDirectiveEnabled)
                    {
                        if (args.Count == tempArgs.Count &&
                            Enumerable.Range(0, args.Count).All(i => args[i] == tempArgs[i]))
                        {
                            ReportTransformation(transformation.Name, null);
                        }
                        else
                        {
                            ReportTransformation(transformation.Name, tempArgs);
                        }
                    }

                    args = tempArgs;
                }
                catch (Exception e)
                {
                    throw new AppRunnerException($"transformation failure for: {transformation}", e);
                }
            }

            return args;
        }

        private void ReportTransformation(string name, Tokens args)
        {
            var maxTokenTypeNameLength = Enum.GetNames(typeof(TokenType)).Max(n => n.Length);

            if (args == null)
            {
                _appSettings.Out.WriteLine($">>> no changes after: {name}");
            }
            else
            {
                _appSettings.Out.WriteLine(name == null ? ">>> from shell" : $">>> transformed after: {name}");
                foreach (var arg in args)
                {
                    var outputFormat = $"  {{0, -{maxTokenTypeNameLength}}}: {{1}}";
                    _appSettings.Out.WriteLine(outputFormat, arg.TokenType, arg.RawValue);
                }
            }
            _appSettings.Out.WriteLine();
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