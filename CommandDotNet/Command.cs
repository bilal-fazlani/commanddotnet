// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    internal class Command : ICommand, ICommandBuilder
    {
        private readonly List<IOption> _options = new List<IOption>();
        private readonly List<IOperand> _operands = new List<IOperand>();
        private readonly List<ICommand> _commands = new List<ICommand>();

        private readonly Dictionary<string, IArgument> _argumentsByAlias = new Dictionary<string, IArgument>();

        public Command(string name, 
            ICustomAttributeProvider customAttributeProvider,
            ICommand parent = null)
        {
            Name = name;
            CustomAttributes = customAttributeProvider;
            Parent = parent;
        }

        public string Name { get; }
        public string Description { get; set; }
        public string ExtendedHelpText { get; set; }

        public IEnumerable<IOperand> Operands => _operands;
        public ICommand Parent { get; }
        public IEnumerable<ICommand> Commands => _commands;
        public ICustomAttributeProvider CustomAttributes { get; }
        public IContextData ContextData { get; } = new ContextData();

        ICommand ICommandBuilder.Command => this;
        
        public void AddSubCommand(ICommand command)
        {
            _commands.Add(command);
        }

        public void AddArgument(IArgument argument)
        {
            RegisterArgumentByAliases(argument);

            if (argument is IOperand operand)
            {
                var lastOperand = Operands.LastOrDefault();
                if (lastOperand != null && lastOperand.Arity.AllowsZeroOrMore())
                {
                    var message =
                        $"The last operand '{lastOperand.Name}' accepts multiple values. No more operands can be added.";
                    throw new AppRunnerException(message);
                }
                _operands.Add(operand);
            }
            if (argument is IOption option)
            {
                _options.Add(option);
            }
        }

        public IEnumerable<IOption> GetOptions(bool includeInherited = true)
        {
            return includeInherited
                ? _options.Concat(this.GetParentCommands().SelectMany(a => a._options.Where(o => o.Inherited)))
                : _options;
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
                ? option
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

        private bool Equals(Command other)
        {
            return string.Equals(Name, other.Name) 
                   && Equals(Parent, other.Parent);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Command) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Parent != null ? Parent.GetHashCode() : 0);
            }
        }
    }
}