// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    internal class Command : ICommand, ICommandBuilder
    {
        private readonly AppSettings _appSettings;
        private Func<int> _invoke;
        private readonly List<IOption> _options = new List<IOption>();
        private readonly List<IOperand> _operands = new List<IOperand>();
        private readonly List<ICommand> _commands = new List<ICommand>();

        private readonly Dictionary<string, IArgument> _argumentsByAlias = new Dictionary<string, IArgument>();

        public Command(
            AppSettings appSettings, 
            string name, 
            ICustomAttributeProvider customAttributeProvider,
            Command parent = null)
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

        public IEnumerable<IOperand> Operands => _operands;
        public ICommand Parent { get; }
        public IEnumerable<ICommand> Commands => _commands;
        public ICustomAttributeProvider CustomAttributeProvider { get; }
        
        public IEnumerable<IOption> GetOptions(bool includeInherited = true)
        {
            return includeInherited
                ? _options.Concat(this.GetParentCommands().SelectMany(a => a._options.Where(o => o.Inherited)))
                : _options;
        }

        public Command AddCommand(string name, ICustomAttributeProvider customAttributeProvider)
        {
            var command = new Command(_appSettings, name, customAttributeProvider, this);
            _commands.Add(command);
            return command;
        }

        internal Option AddOption(string template, string description, IArgumentArity arity, bool inherited,
            string typeDisplayName, object defaultValue, List<string> allowedValues, bool isSystemOption = false)
        {
            var option = new Option(template, arity)
            {
                Description = description,
                Inherited = inherited,
                DefaultValue = defaultValue,
                TypeDisplayName = typeDisplayName,
                AllowedValues = allowedValues,
                IsSystemOption = isSystemOption
            };

            AddArgument(option);
            return option;
        }

        internal Operand AddOperand(
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

            var operand = new Operand
            {
                Name = name, 
                Description = description, 
                Arity = arity,
                TypeDisplayName = typeDisplayName,
                DefaultValue = defaultValue,
                AllowedValues = allowedValues
            };
            AddArgument(operand);
            return operand;
        }

        ICommand ICommandBuilder.Command => this;

        public void AddArgument(IArgument argument)
        {
            RegisterArgumentByAliases(argument);

            if (argument is IOperand operand)
            {
                _operands.Add(operand);
            }
            if (argument is IOption option)
            {
                _options.Add(option);
            }
        }

        public void OnExecute(Func<Task<int>> invoke)
        {
            _invoke = () => invoke().Result;
        }
        
        public int Execute()
        {
            return _invoke();
        }

        public IOption FindOption(string alias)
        {
            return FindOption(this, alias, false) 
                   ?? this.GetParentCommands()
                       .Select(c => FindOption(c, alias, true))
                       .FirstOrDefault(o => o != null);
        }

        private static IOption FindOption(Command command, string alias, bool onlyIfInherited)
        {
            return command._argumentsByAlias.TryGetValue(alias, out var argument)
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

        public override string ToString()
        {
            return $"{nameof(Command)}:{Name} " +
                   $"operands:{_operands.Select(o => o.Name).ToCsv()} " +
                   $"options:{_options.Select(o => o.Name).ToCsv()} " +
                   $"commands:{_commands.Select(c => c.Name).ToCsv()}";
        }
    }
}