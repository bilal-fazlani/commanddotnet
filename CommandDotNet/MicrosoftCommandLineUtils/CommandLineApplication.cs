// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Exceptions;
using CommandDotNet.Extensions;
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
        private readonly List<CommandOption> _options = new List<CommandOption>();
        private readonly List<CommandOperand> _operands = new List<CommandOperand>();
        private readonly List<ICommand> _commands = new List<ICommand>();

        private readonly Dictionary<string, IArgument> _argumentsByAlias = new Dictionary<string, IArgument>();

        public CommandLineApplication(
            AppSettings appSettings, 
            string name, 
            ICustomAttributeProvider customAttributeProvider,
            CommandLineApplication parent = null)
        {
            _appSettings = appSettings;
            Name = name;
            CustomAttributeProvider = customAttributeProvider;
            Parent = parent;
            _invoke = () => 0;
        }

        public string Name { get; }
        public string Description { get; set; }
        public string ExtendedHelpText { get; set; }

        public IOption OptionHelp { get; private set; }
        public IOption OptionVersion { get; private set; }

        public IEnumerable<IOperand> Operands => _operands;
        public ICommand Parent { get; }
        public IEnumerable<ICommand> Commands => _commands;
        public ICustomAttributeProvider CustomAttributeProvider { get; }

        #region Obsolete members

        [Obsolete("do not use.  value is always true.")]
        public bool ShowInHelpText => true;

        [Obsolete("This was used solely for help.  The functionality has been moved to help providers.")]
        public string GetFullCommandName()
        {
            return string.Join(" ", this.GetParentCommands(true).Reverse().Select(c => c.Name));
        }

        #endregion

        public IEnumerable<IOption> GetOptions(bool includeInherited = true)
        {
            return includeInherited
                ? _options.Concat(this.GetParentCommands().SelectMany(a => a._options.Where(o => o.Inherited)))
                : _options;
        }

        public CommandLineApplication Command(string name, ICustomAttributeProvider customAttributeProvider)
        {
            var command = new CommandLineApplication(_appSettings, name, customAttributeProvider, this);
            _commands.Add(command);
            return command;
        }

        internal CommandOption Option(string template, string description, IArgumentArity arity, bool inherited,
            string typeDisplayName, object defaultValue, List<string> allowedValues
            )
        {
            var option = new CommandOption(template, arity)
            {
                Description = description,
                Inherited = inherited,
                DefaultValue = defaultValue,
                TypeDisplayName = typeDisplayName,
                AllowedValues = allowedValues
            };

            RegisterArgumentByAliases(option);

            _options.Add(option);
            return option;
        }

        public CommandOperand Operand(
            string name, string description, IArgumentArity arity,
            string typeDisplayName, object defaultValue, List<string> allowedValues)
        {
            var lastOperand = Operands.LastOrDefault();
            if (lastOperand != null && lastOperand.Arity.AllowsZeroOrMore())
            {
                var message =
                    $"The last operand '{lastOperand.Name}' accepts multiple values. No more operands can be added.";
                throw new AppRunnerException(message);
            }

            var operand = new CommandOperand
            {
                Name = name, 
                Description = description, 
                Arity = arity,
                TypeDisplayName = typeDisplayName,
                DefaultValue = defaultValue,
                AllowedValues = allowedValues
            };

            RegisterArgumentByAliases(operand);

            _operands.Add(operand);
            return operand;
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

        public IOption FindOption(string alias)
        {
            return FindOption(this, alias, false) 
                   ?? this.GetParentCommands()
                       .Select(c => FindOption(c, alias, true))
                       .FirstOrDefault(o => o != null);
        }

        // Helper method that adds a help option
        public void HelpOption(string template)
        {
            // Help option is special because we stop parsing once we see it
            // So we store it separately for further use
            OptionHelp = Option(template, "Show help information", ArgumentArity.Zero, false, Constants.TypeDisplayNames.Flag, DBNull.Value, null);
            OptionHelp.IsSystemOption = true;
        }

        internal void VersionOption(string template, Action printVersion)
        {
            // Version option is special because we stop parsing once we see it
            // So we store it separately for further use
            OptionVersion = Option(template, "Show version information", ArgumentArity.Zero, false, Constants.TypeDisplayNames.Flag, DBNull.Value, null);
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

        private static IOption FindOption(CommandLineApplication app, string alias, bool onlyIfInherited)
        {
            return app._argumentsByAlias.TryGetValue(alias, out var argument)
                   && (argument is IOption option)
                   && (!onlyIfInherited || option.Inherited)
                ? (IOption)argument
                : null;
        }

        private void RegisterArgumentByAliases(IArgument argument)
        {
            foreach (var parent in this.GetParentCommands(includeCurrent: true))
            {
                IArgument duplicatedArg = null;
                var duplicateAlias = argument.Aliases.FirstOrDefault(a => _argumentsByAlias.TryGetValue(a, out duplicatedArg));

                // the alias cannot duplicate any argument in this command or any inherited option from parent commands
                if (duplicateAlias != null && (ReferenceEquals(parent, this) || (duplicatedArg is IOption option && option.Inherited)))
                {
                    throw new AppRunnerException(
                        $"Duplicate alias detected. Attempted to add `{argument}` but `{duplicatedArg}` already exists");
                }
            }

            argument.Aliases.ForEach(a => _argumentsByAlias.Add(a, argument));
        }
    }
}