// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Exceptions;
using CommandDotNet.HelpGeneration;
using CommandDotNet.Models;
using CommandDotNet.Parsing;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    internal class CommandLineApplication : ICommand
    {
        private readonly AppSettings _appSettings;

        private readonly List<string> _remainingArguments;
        private Action _printVersion;
        private Func<int> _invoke;
        private CommandOption _optionVersion;
        private CommandLineApplication _parent;

        // Indicates whether the parser should throw an exception when it runs into an unexpected argument.
        // If this field is set to false, the parser will stop parsing when it sees an unexpected argument, and all
        // remaining arguments, including the first unexpected argument, will be stored in RemainingArguments property.
        public CommandLineApplication(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _remainingArguments = new List<string>();
            _invoke = () => 0;

            Options = new HashSet<CommandOption>();
            Arguments = new HashSet<CommandArgument>();
            Commands = new List<ICommand>();
        }

        public string Name { get; set; }
        public string Syntax { get; set; }
        public string Description { get; set; }
        public bool ShowInHelpText => true;
        public string ExtendedHelpText { get; set; }
        public HashSet<CommandOption> Options { get; }
        public CommandOption OptionHelp { get; private set; }
        public ICustomAttributeProvider CustomAttributeProvider { get; set; }
        public HashSet<CommandArgument> Arguments { get; }
        public ICommand Parent => _parent;
        public List<ICommand> Commands { get; }

        [Obsolete("This was used solely for help.  The functionality has been moved to help providers.")]

        public string GetFullCommandName()
        {
            return string.Join(" ", this.GetParentCommands(true).Reverse().Select(c => c.Name));
        }

        public IEnumerable<CommandOption> GetOptions()
        {
            var inheritedOptions = this.GetParentCommands().SelectMany(a => a.Options.Where(o => o.Inherited));
            return Options.Concat(inheritedOptions);
        }

        private CommandOption FindOption(Func<CommandOption, string> optionNameToCompare, string optionName)
        {
            return GetOptions().SingleOrDefault(o =>
                string.Equals(optionNameToCompare(o), optionName, StringComparison.Ordinal));
        }

        public CommandLineApplication Command(string name)
        {
            var command = new CommandLineApplication(_appSettings) { Name = name, _parent = this };
            Commands.Add(command);
            return command;
        }

        internal CommandOption Option(string template, string description, CommandOptionType optionType, 
            Action<CommandOption> configuration, bool inherited,
            string typeDisplayName, object defaultValue, bool multiple, List<string> allowedValues
            )
        {
            var option = new CommandOption(template, optionType)
            {
                Description = description,
                Inherited = inherited,
                DefaultValue = defaultValue,
                TypeDisplayName = typeDisplayName,
                Multiple = multiple,
                AllowedValues = allowedValues
            };
            bool optionAdded = Options.Add(option);
            if(!optionAdded)
                throw new AppRunnerException($"Option with template `{option.Template}` already added");
            configuration(option);
            return option;
        }

        public CommandArgument Argument(
            string name, string description, 
            Action<CommandArgument> configuration,
            string typeDisplayName, object defaultValue, bool multiple,
            List<string> allowedValues)
        {
            var lastArg = Arguments.LastOrDefault();
            if (lastArg != null && lastArg.MultipleValues)
            {
                var message =
                    $"The last argument '{lastArg.Name}' accepts multiple values. No more argument can be added.";
                throw new AppRunnerException(message);
            }

            var argument = new CommandArgument
            {
                Name = name, 
                Description = description, 
                MultipleValues = multiple,
                TypeDisplayName = typeDisplayName,
                DefaultValue = defaultValue,
                AllowedValues = allowedValues
            };
            bool argumentAdded = Arguments.Add(argument);
            if(!argumentAdded)
                throw new AppRunnerException($"Argument with name '{argument.Name}' already added");
            configuration(argument);
            return argument;
        }

        public void OnExecute(Func<Task<int>> invoke)
        {
            _invoke = () => invoke().Result;
        }
        
        public int Execute(ParserContext parserContext, string[] args)
        {
            var directivesResult = Directives.ProcessDirectives(_appSettings, ref args);
            if (directivesResult.ExitCode.HasValue)
            {
                return directivesResult.ExitCode.Value;
            }
            var command = ParseCommand(parserContext, args);
            return command._invoke();
        }

        private CommandLineApplication ParseCommand(ParserContext parserContext, string[] args)
        {
            CommandLineApplication command = this;
            CommandOption option = null;
            IEnumerator<CommandArgument> arguments = null;
            
            // TODO: when Parse directive is enabled, log the args after each transformation

            /*
            _appSettings.Out.WriteLine("received:");
            foreach (var arg in args)
            {
                _appSettings.Out.WriteLine($"   {arg}");
            }
            _appSettings.Out.WriteLine();
            */

            foreach (var transformation in parserContext.ArgumentTransformations.OrderBy(t => t.Order))
            {
                try
                {
                    args = transformation.Transformation(args);

                    /*
                    _appSettings.Out.WriteLine($"transformation: {transformation.Name}");
                    foreach (var arg in args)
                    {
                        _appSettings.Out.WriteLine($"   {arg}");
                    }
                    _appSettings.Out.WriteLine();
                    */
                }
                catch (Exception e)
                {
                    throw new AppRunnerException($"transformation failure for: {transformation}", e);
                }
            }

            for (var index = 0; index < args.Length; index++)
            {
                var arg = args[index];

                /* Process flow
                 *
                 * easy to determine options so check those first.  options start with -- or -
                 * if option,
                 *   check if the value is provided in same string with : or =
                 *     if value is needed and not provided store option for next argument
                 *
                 * if previous argument was option needing a value,
                 *   parse argument for option
                 *
                 * if argument is the name of a subcommand
                 *   processing remaining arguments for the sub command
                 *
                 * assign argument as value for next argument in the command
                 */

                // control flow is tricky.  Watch for: continue, break, return & throw

                if (option == null)
                {
                    string[] optionParts = null;

                    var isLongOption = arg.StartsWith("--");
                    var isShortOption = !isLongOption && arg.StartsWith("-");

                    if (isLongOption)
                    {
                        optionParts = arg.Substring(2).Split(new[] {':', '='}, 2);
                        var optionName = optionParts[0];
                        option = command.FindOption(o => o.LongName, optionName);

                        if (option == null)
                        {
                            var isArgumentSeparator = string.IsNullOrEmpty(optionName);

                            // ??? if AllowArgumentSeparator is true, why would this be considered an unexpected argument?
                            if (isArgumentSeparator && !_appSettings.ThrowOnUnexpectedArgument &&
                                _appSettings.AllowArgumentSeparator)
                            {
                                // skip over the '--' argument separator
                                // all remaining arguments to be added to command.RemainingArguments 
                                index++;
                            }

                            HandleUnexpectedArg(command, args, index, argTypeName: "option");
                            break;
                        }
                    }

                    if (isShortOption)
                    {
                        optionParts = arg.Substring(1).Split(new[] {':', '='}, 2);
                        var optionName = optionParts[0];
                        // If not a short option, try symbol option.  e.g.  ?
                        option = command.FindOption(o => o.ShortName, optionName)
                                 ?? command.FindOption(o => o.SymbolName, optionName);

                        if (option == null)
                        {
                            HandleUnexpectedArg(command, args, index, argTypeName: "option");
                            break;
                        }
                    }

                    if (isLongOption || isShortOption)
                    {
                        // If we find a help/version option, show information and stop parsing
                        if (Equals(command.OptionHelp, option))
                        {
                            command._invoke = () =>
                            {
                                command.ShowHelp();
                                return 0;
                            };
                            return command;
                        }

                        if (Equals(command._optionVersion, option))
                        {
                            command._invoke = () =>
                            {
                                command.ShowVersion();
                                return 0;
                            };
                            return command;
                        }

                        if (optionParts.Length == 2)
                        {
                            var optionValue = optionParts[1];
                            if (!option.TryParse(optionValue))
                            {
                                command.ShowHint();
                                throw new CommandParsingException(command,
                                    $"Unexpected value '{optionValue}' for option '{option.LongName}'");
                            }

                            option = null;
                        }
                        else if (option.OptionType == CommandOptionType.NoValue)
                        {
                            // No value is needed for this option
                            option.TryParse(null);
                            option = null;
                        }

                        // process next argument
                        continue;
                    }
                }

                if (option != null) // this is the value for the previous option
                {
                    if (!option.TryParse(arg))
                    {
                        command.ShowHint();
                        throw new CommandParsingException(command, $"Unexpected value '{arg}' for option '{option.LongName}'");
                    }

                    option = null;

                    // process next argument
                    continue;
                }

                if (arguments == null)
                {
                    var subCommand = command.Commands
                        .Cast<CommandLineApplication>()
                        .FirstOrDefault(c => c.Name.Equals(arg, StringComparison.OrdinalIgnoreCase));

                    if (subCommand != null)
                    {
                        command = subCommand;

                        // process next argument for the subcommand
                        continue;
                    }
                }

                if (arguments == null)
                {
                    // TODO: Arguments is expected to be ordered but HashSet does not guarantee order
                    arguments = new CommandArgumentEnumerator(command.Arguments.GetEnumerator());
                }

                if (arguments.MoveNext())
                {
                    arguments.Current.Values.Add(arg);
                }
                else
                {
                    HandleUnexpectedArg(command, args, index, argTypeName: "command or argument");
                    break;
                }
            }

            if (option != null) // an option was left without a value
            {
                command.ShowHint();
                throw new CommandParsingException(command, $"Missing value for option '{option.LongName}'");
            }

            return command;
        }

        // Helper method that adds a help option
        public void HelpOption(string template)
        {
            // Help option is special because we stop parsing once we see it
            // So we store it separately for further use
            OptionHelp = Option(template, "Show help information", CommandOptionType.NoValue, _=>{}, false, Constants.TypeDisplayNames.Flag, DBNull.Value, false, null);
            OptionHelp.IsSystemOption = true;
        }

        internal void VersionOption(string template, Action printVersion)
        {
            // Version option is special because we stop parsing once we see it
            // So we store it separately for further use
            _optionVersion = Option(template, "Show version information", CommandOptionType.NoValue, _=>{}, false, Constants.TypeDisplayNames.Flag, DBNull.Value, false, null);
            _optionVersion.IsSystemOption = true;
            _printVersion = printVersion;
        }

        // Helper method that adds a version option

        // Show short hint that reminds users to use help option
        private void ShowHint()
        {
            if (OptionHelp != null)
            {
                _appSettings.Out.WriteLine($"Specify --{OptionHelp.LongName} for a list of available options and commands.");
            }
        }

        // Show full help
        public void ShowHelp()
        {
            IHelpProvider helpTextProvider = HelpTextProviderFactory.Create(_appSettings);
            _appSettings.Out.WriteLine(helpTextProvider.GetHelpText(this));
        }

        private void ShowVersion()
        {
            _printVersion();
        }

        private void HandleUnexpectedArg(CommandLineApplication command, string[] args, int index, string argTypeName)
        {
            if (_appSettings.ThrowOnUnexpectedArgument)
            {
                command.ShowHint();
                throw new CommandParsingException(command, $"Unrecognized {argTypeName} '{args[index]}'");
            }

            // All remaining arguments are stored for further use
            command._remainingArguments.AddRange(new ArraySegment<string>(args, index, args.Length - index));
        }

        private class CommandArgumentEnumerator : IEnumerator<CommandArgument>
        {
            private readonly IEnumerator<CommandArgument> _enumerator;

            public CommandArgumentEnumerator(IEnumerator<CommandArgument> enumerator)
            {
                _enumerator = enumerator;
            }

            public CommandArgument Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (Current == null || !Current.MultipleValues)
                {
                    return _enumerator.MoveNext();
                }

                // If current argument allows multiple values, we don't move forward and
                // all later values will be added to current CommandArgument.Values
                return true;
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }
    }
}