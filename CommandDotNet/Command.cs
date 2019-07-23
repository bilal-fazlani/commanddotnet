// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public class Command : ICommandBuilder, INameAndDescription
    {
        private readonly List<Option> _options = new List<Option>();
        private readonly List<Operand> _operands = new List<Operand>();
        private readonly List<Command> _commands = new List<Command>();

        private readonly Dictionary<string, IArgument> _argumentsByAlias = new Dictionary<string, IArgument>();

        public Command(string name, 
            ICustomAttributeProvider customAttributeProvider,
            Command parent = null)
        {
            Name = name;
            CustomAttributes = customAttributeProvider;
            Parent = parent;
        }

        public string Name { get; }
        public string Description { get; set; }
        public string ExtendedHelpText { get; set; }

        public IEnumerable<Operand> Operands => _operands;
        public Command Parent { get; }
        public IEnumerable<Command> Commands => _commands;
        public ICustomAttributeProvider CustomAttributes { get; }
        public IContextData ContextData { get; } = new ContextData();

        Command ICommandBuilder.Command => this;
        
        public void AddSubCommand(Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _commands.Add(command);
        }

        public void AddArgument(IArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            switch (argument)
            {
                case Operand operand:
                    AddOperand(operand);
                    break;
                case Option option:
                    AddOption(option);
                    break;
                default:
                    throw new ArgumentException(
                        $"argument type must be `{typeof(Operand)}` or `{typeof(Option)}` but was `{argument.GetType()}`. " +
                        $"If `{argument.GetType()}` was created for extensibility, " +
                        $"consider using {nameof(ContextData)} to store service classes instead.");
            }

            RegisterArgumentByAliases(argument);
        }

        public IEnumerable<Option> GetOptions(bool includeInherited = true)
        {
            return includeInherited
                ? _options.Concat(this.GetParentCommands().SelectMany(a => a._options.Where(o => o.Inherited)))
                : _options;
        }

        public Option FindOption(string alias)
        {
            if (alias == null)
            {
                throw new ArgumentNullException(nameof(alias));
            }

            return FindOption(this, alias, false)
                   ?? this.GetParentCommands()
                       .Select(c => FindOption(c, alias, true))
                       .FirstOrDefault(o => o != null);
        }

        private static Option FindOption(Command command, string alias, bool onlyIfInherited)
        {
            return command._argumentsByAlias.TryGetValue(alias, out var argument)
                   && (argument is Option option)
                   && (!onlyIfInherited || option.Inherited)
                ? option
                : null;
        }

        private void AddOperand(Operand operand)
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

        private void AddOption(Option option)
        {
            _options.Add(option);
        }

        private void RegisterArgumentByAliases(IArgument argument)
        {
            foreach (var parent in this.GetParentCommands(includeCurrent: true))
            {
                IArgument duplicatedArg = null;
                var duplicateAlias = argument.Aliases.FirstOrDefault(a => _argumentsByAlias.TryGetValue(a, out duplicatedArg));

                // the alias cannot duplicate any argument in this command or any inherited option from parent commands
                if (duplicateAlias != null && (ReferenceEquals(parent, this) || (duplicatedArg is Option option && option.Inherited)))
                {
                    throw new AppRunnerException(
                        $"Duplicate alias detected. Attempted to add `{argument}` to `{this}` but `{duplicatedArg}` already exists");
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