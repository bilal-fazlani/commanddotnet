// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
        private Action _printVersion;
        private Func<int> _invoke;
        private CommandLineApplication _parent;

        // Indicates whether the parser should throw an exception when it runs into an unexpected argument.
        // If this field is set to false, the parser will stop parsing when it sees an unexpected argument, and all
        // remaining arguments, including the first unexpected argument, will be stored in RemainingArguments property.
        public CommandLineApplication(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _invoke = () => 0;

            Options = new HashSet<CommandOption>();
            Arguments = new HashSet<CommandArgument>();
            Commands = new List<ICommand>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShowInHelpText => true;
        public string ExtendedHelpText { get; set; }
        public HashSet<CommandOption> Options { get; }
        public CommandOption OptionHelp { get; private set; }
        public CommandOption OptionVersion { get; private set; }
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
            var directivesResult = Directives.ProcessDirectives(_appSettings, parserContext, ref args);
            if (directivesResult.ExitCode.HasValue)
            {
                return directivesResult.ExitCode.Value;
            }

            var parseResult = new CommandParser(_appSettings, parserContext).ParseCommand(this, args);

            // if the ExitCode has been set, the parser has determined
            // the command should not be executed
            if (parseResult.ExitCode.HasValue)
            {
                return parseResult.ExitCode.Value;
            }
            return ((CommandLineApplication)parseResult.Command)._invoke();
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
            OptionVersion = Option(template, "Show version information", CommandOptionType.NoValue, _=>{}, false, Constants.TypeDisplayNames.Flag, DBNull.Value, false, null);
            OptionVersion.IsSystemOption = true;
            _printVersion = printVersion;
        }

        // Helper method that adds a version option

        // Show full help
        public void ShowHelp()
        {
            IHelpProvider helpTextProvider = HelpTextProviderFactory.Create(_appSettings);
            _appSettings.Out.WriteLine(helpTextProvider.GetHelpText(this));
        }

        public void ShowVersion()
        {
            this.GetRootCommand()._printVersion();
        }
    }
}