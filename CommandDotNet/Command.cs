// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public class Command : INameAndDescription
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

        /// <summary>The <see cref="Operand"/>s for this <see cref="Command"/></summary>
        public IReadOnlyCollection<Operand> Operands => _operands.AsReadOnly();
        
        /// <summary>The <see cref="Option"/>s for this <see cref="Command"/></summary>
        public IReadOnlyCollection<Option> Options => _options.AsReadOnly();

        /// <summary>
        /// The <see cref="Command"/> that hosts this <see cref="Command"/>.
        /// Is null for the root command. Some parent commands are not executable
        /// but are defined only to group for other commands.
        /// </summary>
        public Command Parent { get; }

        /// <summary>The <see cref="Command"/>s that can be accessed via this <see cref="Command"/></summary>
        public IReadOnlyCollection<Command> Subcommands => _commands.AsReadOnly();

        /// <summary>The attributes defined on the method or class that define this <see cref="Command"/></summary>
        public ICustomAttributeProvider CustomAttributes { get; }

        /// <summary>The services used by middleware and associated with this <see cref="Command"/></summary>
        public IServices Services { get; } = new Services();

        /// <summary>Returns the option for the given alias, if it exists.</summary>
        public Option FindOption(string alias)
        {
            if (alias == null)
            {
                throw new ArgumentNullException(nameof(alias));
            }

            return this._argumentsByAlias.TryGetValue(alias, out var argument)
                   && (argument is Option option)
                ? option
                : null;
        }

        internal void AddSubCommand(Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _commands.Add(command);
        }

        internal void AddArgument(IArgument argument)
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
                        $"consider using {nameof(Services)} to store service classes instead.");
            }

            RegisterArgumentByAliases(argument);
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