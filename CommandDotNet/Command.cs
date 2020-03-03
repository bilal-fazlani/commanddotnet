// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    public sealed class Command : IArgumentNode
    {
        private readonly List<Option> _options = new List<Option>();
        private readonly List<Option> _optionsForExecutableSubcommands = new List<Option>();
        private readonly List<Operand> _operands = new List<Operand>();
        private readonly List<Command> _subcommands = new List<Command>();

        private readonly Dictionary<string, IArgumentNode> _argumentsByAlias = new Dictionary<string, IArgumentNode>();

        public Command(string name, 
            ICustomAttributeProvider customAttributeProvider = null,
            bool isExecutable = true,
            Command parent = null,
            string definitionSource = null)
        {
            Name = name.IsNullOrEmpty() 
                ? throw new ArgumentNullException(name) 
                : name;
            CustomAttributes = customAttributeProvider ?? NullCustomAttributeProvider.Instance;
            IsExecutable = isExecutable;
            Parent = parent;
            DefinitionSource = definitionSource;
            Aliases = new[] {name};
        }

        public string Name { get; }
        public string Description { get; set; }
        public string Usage { get; set; }
        public string ExtendedHelpText { get; set; }

        /// <summary>
        /// When true, the command can be executed.<br/>
        /// When false, a subcommand must be specified.<br/>
        /// The final command specified must be an executable command must be.
        /// </summary>
        public bool IsExecutable { get; }

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

        /// <summary>The aliases defined for this command</summary>
        public IReadOnlyCollection<string> Aliases { get; }

        /// <summary>The source that defined this command</summary>
        public string DefinitionSource { get; }

        /// <summary>The <see cref="Command"/>s that can be accessed via this <see cref="Command"/></summary>
        public IReadOnlyCollection<Command> Subcommands => _subcommands.AsReadOnly();

        /// <summary>The attributes defined on the method or class that define this <see cref="Command"/></summary>
        public ICustomAttributeProvider CustomAttributes { get; }

        /// <summary>The services used by middleware and associated with this <see cref="Command"/></summary>
        public IServices Services { get; } = new Services();

        public IArgumentNode FindArgumentNode(string alias) => 
            _argumentsByAlias.GetValueOrDefault(alias ?? throw new ArgumentNullException(nameof(alias)));

        /// <summary>Returns the option for the given alias, if it exists.</summary>
        public Option FindOption(string alias) =>
            FindArgumentNode(alias) is Option option ? option : null;

        internal void AddArgumentNode(IArgumentNode argumentNode)
        {
            if (argumentNode == null)
            {
                throw new ArgumentNullException(nameof(argumentNode));
            }

            switch (argumentNode)
            {
                case Command command:
                    AddSubcommand(command);
                    break;
                case Operand operand:
                    AddOperand(operand);
                    break;
                case Option option:
                    AddOption(option);
                    break;
                default:
                    throw new ArgumentException(
                        $"argument node type must be `{typeof(Command)}`, `{typeof(Operand)}` or `{typeof(Option)}` but was `{argumentNode.GetType()}`. " +
                        $"If `{argumentNode.GetType()}` was created for extensibility, " +
                        $"consider using {nameof(Services)} to store service classes instead.");
            }
        }

        private void AddSubcommand(Command command)
        {
            _subcommands.Add(command);
            RegisterArgumentByAliases(command);
            if (command.IsExecutable)
            {
                this.GetParentCommands(includeCurrent: true)
                    .SelectMany(c => c._optionsForExecutableSubcommands)
                    .ForEach(command.AddArgumentNode);
            }
        }

        private void AddOperand(Operand operand)
        {
            var lastOperand = Operands.LastOrDefault();
            if (lastOperand != null && lastOperand.Arity.AllowsMany())
            {
                var message =
                    $"The last operand '{lastOperand.Name}' accepts multiple values. No more operands can be added.";
                throw new AppRunnerException(message);
            }
            _operands.Add(operand);
            RegisterArgumentByAliases(operand);
        }

        private void AddOption(Option option)
        {
            if (option.AssignToExecutableSubcommands)
            {
                if (option.Parent == this)
                {
                    _optionsForExecutableSubcommands.Add(option);
                    this.GetDescendentCommands(includeCurrent: false)
                        .Where(c => c.IsExecutable)
                        .ForEach(c => c.AddArgumentNode(option));
                }
                if(IsExecutable)
                {
                    _options.Add(option);
                    RegisterArgumentByAliases(option);
                }
            }
            else
            {
                _options.Add(option);
                RegisterArgumentByAliases(option);
            }
        }

        private void RegisterArgumentByAliases(IArgumentNode argumentNode)
        {
            foreach (var parentOrThis in this.GetParentCommands(includeCurrent: true))
            {
                IArgumentNode duplicatedArg = null;
                var duplicateAlias = argumentNode.Aliases.FirstOrDefault(a => _argumentsByAlias.TryGetValue(a, out duplicatedArg));

                if (duplicateAlias != null && ReferenceEquals(parentOrThis, this))
                {
                    string GetArgNodeName(IArgumentNode arg) => $"{arg.GetType().Name}:{arg.Name}(source:{arg.DefinitionSource})";

                    throw new InvalidConfigurationException(
                        $"Duplicate alias '{duplicateAlias}' added to command '{this.Name}'. " +
                        $"Duplicates: '{GetArgNodeName(argumentNode)}' & '{GetArgNodeName(duplicatedArg)}'");
                }
            }

            argumentNode.Aliases.ForEach(a => _argumentsByAlias.Add(a, argumentNode));
        }

        public override string ToString()
        {
            return $"{nameof(Command)}:{Name} " +
                   $"operands:{_operands.Select(o => o.Name).ToCsv()} " +
                   $"options:{_options.Select(o => o.Name).ToCsv()} " +
                   $"commands:{_subcommands.Select(c => c.Name).ToCsv()}";
        }

        public static bool operator ==(Command x, Command y) => (object) x == (object) y;

        public static bool operator !=(Command x, Command y) => !(x == y);

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