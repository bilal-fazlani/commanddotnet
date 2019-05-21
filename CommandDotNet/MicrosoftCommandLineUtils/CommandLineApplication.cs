// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Exceptions;
using CommandDotNet.HelpGeneration;
using CommandDotNet.Models;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    internal class CommandLineApplication : ICommand
    {
        private readonly AppSettings _appSettings;
        // Indicates whether the parser should throw an exception when it runs into an unexpected argument.
        // If this field is set to false, the parser will stop parsing when it sees an unexpected argument, and all
        // remaining arguments, including the first unexpected argument, will be stored in RemainingArguments property.
        public CommandLineApplication(AppSettings appSettings)
        {
            _appSettings = appSettings;
            Options = new HashSet<CommandOption>();
            Arguments = new HashSet<CommandArgument>();
            Commands = new List<ICommand>();
            RemainingArguments = new List<string>();
            Invoke = () => 0;
        }

        public CommandLineApplication Parent { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Syntax { get; set; }
        public string Description { get; set; }
        public bool ShowInHelpText { get; set; } = true;
        public string ExtendedHelpText { get; set; }
        public HashSet<CommandOption> Options { get; }
        public CommandOption OptionHelp { get; private set; }
        public CommandOption OptionVersion { get; private set; }
        public HashSet<CommandArgument> Arguments { get; }
        public readonly List<string> RemainingArguments;
        public Func<int> Invoke { get; set; }
        public Func<string> LongVersionGetter { get; set; }
        public Func<string> ShortVersionGetter { get; set; }
        public List<ICommand> Commands { get; }

        public string GetFullCommandName()
        {
            return string.Join(" ", GetSelfAndParentCommands().Reverse().Select(c => c.Name));
        } 

        public IEnumerable<CommandOption> GetOptions()
        {
            var inheritedOptions = GetParentCommands().SelectMany(a => a.Options.Where(o => o.Inherited));
            return Options.Concat(inheritedOptions);
        }

        private CommandOption FindOption(Func<CommandOption, string> optionNameToCompare, string optionName)
        {
            return GetOptions().SingleOrDefault(o =>
                string.Equals(optionNameToCompare(o), optionName, StringComparison.Ordinal));
        }

        public CommandLineApplication Command(string name, Action<CommandLineApplication> configuration)
        {
            var command = new CommandLineApplication(_appSettings) { Name = name, Parent = this };
            Commands.Add(command);
            configuration(command);
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
            Invoke = () => invoke().Result;
        }
        
        public int Execute(params string[] args)
        {
            CommandLineApplication command = this;
            CommandOption option = null;
            IEnumerator<CommandArgument> arguments = null;

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
                        optionParts = arg.Substring(2).Split(new[] { ':', '=' }, 2);
                        var optionName = optionParts[0];
                        option = command.FindOption(o => o.LongName, optionName);

                        if (option == null)
                        {
                            var isArgumentSeparator = string.IsNullOrEmpty(optionName);
                            
                            // ??? if AllowArgumentSeparator is true, why would this be considered an unexpected argument?
                            if (isArgumentSeparator && !_appSettings.ThrowOnUnexpectedArgument  && _appSettings.AllowArgumentSeparator)
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
                        optionParts = arg.Substring(1).Split(new[] { ':', '=' }, 2);
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
                            command.ShowHelp();
                            return 0;
                        }
                        if (Equals(command.OptionVersion, option))
                        {
                            command.ShowVersion();
                            return 0;
                        }

                        if (optionParts.Length == 2)
                        {
                            var optionValue = optionParts[1];
                            if (!option.TryParse(optionValue))
                            {
                                command.ShowHint();
                                throw new CommandParsingException(command, $"Unexpected value '{optionValue}' for option '{option.LongName}'");
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

            return command.Invoke();
        }

        // Helper method that adds a help option
        public void HelpOption(string template)
        {
            // Help option is special because we stop parsing once we see it
            // So we store it separately for further use
            OptionHelp = Option(template, "Show help information", CommandOptionType.NoValue, _=>{}, false, Constants.TypeDisplayNames.Flag, DBNull.Value, false, null);
        }

        public void VersionOption(string template,
            string shortFormVersion,
            string longFormVersion = null)
        {
            if (longFormVersion == null)
            {
                VersionOption(template, () => shortFormVersion);
            }
            else
            {
                VersionOption(template, () => shortFormVersion, () => longFormVersion);
            }
        }

        // Helper method that adds a version option
        public CommandOption VersionOption(string template,
            Func<string> shortFormVersionGetter,
            Func<string> longFormVersionGetter = null)
        {
            // Version option is special because we stop parsing once we see it
            // So we store it separately for further use
            OptionVersion = Option(template, "Show version information", CommandOptionType.NoValue, _=>{}, false, Constants.TypeDisplayNames.Flag, DBNull.Value, false, null);
            ShortVersionGetter = shortFormVersionGetter;
            LongVersionGetter = longFormVersionGetter ?? shortFormVersionGetter;

            return OptionVersion;
        }

        // Show short hint that reminds users to use help option
        public void ShowHint()
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

        public void ShowVersion()
        {
            _appSettings.Out.WriteLine(FullName);
            _appSettings.Out.WriteLine(LongVersionGetter());
        }

        private IEnumerable<CommandLineApplication> GetSelfAndParentCommands()
        {
            for (CommandLineApplication c = this; c != null; c = c.Parent)
            {
                yield return c;
            }
        }
 
        private IEnumerable<CommandLineApplication> GetParentCommands()
        {
            if (Parent == null)
            {
                yield break;
            }
            for (CommandLineApplication c = Parent; c != null; c = c.Parent)
            {
                yield return c;
            }
        }

        private void HandleUnexpectedArg(CommandLineApplication command, string[] args, int index, string argTypeName)
        {
            if (_appSettings.ThrowOnUnexpectedArgument)
            {
                command.ShowHint();
                throw new CommandParsingException(command, $"Unrecognized {argTypeName} '{args[index]}'");
            }

            // All remaining arguments are stored for further use
            command.RemainingArguments.AddRange(new ArraySegment<string>(args, index, args.Length - index));
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