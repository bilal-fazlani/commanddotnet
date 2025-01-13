﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Builders;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;
using JetBrains.Annotations;

namespace CommandDotNet;

[PublicAPI]
public sealed class Command : IArgumentNode
{
    private Command? _parent;
    private readonly List<Option> _options = [];
    private readonly List<Option> _optionsForExecutableSubcommands = [];
    private readonly List<Operand> _operands = [];
    private readonly List<Command> _subcommands = [];

    private readonly Dictionary<string, IArgumentNode> _argumentsByAlias = new();

    [PublicAPI]
    public Command(string name, 
        ICustomAttributeProvider? customAttributeProvider = null,
        bool isExecutable = true,
        Command? parent = null,
        string? definitionSource = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
            
        Name = name;
        CustomAttributes = customAttributeProvider ?? NullCustomAttributeProvider.Instance;
        IsExecutable = isExecutable;
        if (parent is not null)
        {
            Parent = parent;
        }
        DefinitionSource = definitionSource;
        Aliases = [name];
    }

    public string Name { get; }
    public string? Description { get; set; }
    public string? Usage { get; set; }
    public string? ExtendedHelpText { get; set; }

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
    public Command? Parent
    {
        get => _parent;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException($"{nameof(Parent)} cannot be assigned to null");
            }
            if (_parent is null)
            {
                _parent = value;
            }
            else if (_parent != value)
            {
                throw new InvalidConfigurationException($"{nameof(Parent)} is already assigned for {this}.  Current={_parent} New={value}");
            }
        }
    }

    /// <summary>The aliases defined for this command</summary>
    public IReadOnlyCollection<string> Aliases { get; }

    /// <summary>
    /// Overrides <see cref="ParseAppSettings.IgnoreUnexpectedOperands"/><br/>
    /// When false, unexpected operands will generate a parse failure.<br/>
    /// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
    /// </summary>
    public bool IgnoreUnexpectedOperands { get; set; }

    /// <summary>
    /// The <see cref="ArgumentSeparatorStrategy"/>.
    /// This can be set in the <see cref="CommandAttribute"/>
    /// </summary>
    public ArgumentSeparatorStrategy ArgumentSeparatorStrategy { get; set; }

    /// <summary>The source that defined this command</summary>
    public string? DefinitionSource { get; }

    /// <summary>The <see cref="Command"/>s that can be accessed via this <see cref="Command"/></summary>
    public IReadOnlyCollection<Command> Subcommands => _subcommands.AsReadOnly();

    /// <summary>The attributes defined on the method or class that define this <see cref="Command"/></summary>
    public ICustomAttributeProvider CustomAttributes { get; }

    /// <summary>The services used by middleware and associated with this <see cref="Command"/></summary>
    public IServices Services { get; } = new Services();

    public IArgumentNode? FindArgumentNode(string alias) => 
        _argumentsByAlias.GetValueOrDefault(alias ?? throw new ArgumentNullException(nameof(alias)));

    public TArgumentNode? Find<TArgumentNode>(string alias)
        where TArgumentNode : class, IArgumentNode => 
        (TArgumentNode?) FindArgumentNode(alias);

    internal void AddArgumentNode(IArgumentNode argumentNode)
    {
        ArgumentNullException.ThrowIfNull(argumentNode);

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
        command.Parent = this;
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
        operand.Parent = this;
        var lastOperand = Operands.LastOrDefault();
        if (lastOperand is not null && lastOperand.Arity.AllowsMany())
        {
            var message =
                $"The last operand '{lastOperand.Name}' accepts multiple values. No more operands can be added.";
            throw new InvalidConfigurationException(message);
        }
        _operands.Add(operand);
        RegisterArgumentByAliases(operand);
    }

    private void AddOption(Option option)
    {
        if (!option.AssignToExecutableSubcommands || option.Parent is null)
        {
            option.Parent = this;
        }

        if (option.AssignToExecutableSubcommands)
        {
            if (option.Parent == this)
            {
                _optionsForExecutableSubcommands.Add(option);
                this.GetDescendentCommands(includeCurrent: false)
                    .Where(c => c.IsExecutable)
                    .ForEach(c => c.AddOption(option));
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
            IArgumentNode? duplicatedArg = null;
            var duplicateAlias = argumentNode.Aliases.FirstOrDefault(a => _argumentsByAlias.TryGetValue(a, out duplicatedArg));

            if (duplicateAlias != null && ReferenceEquals(parentOrThis, this))
            {
                throw new InvalidConfigurationException(
                    $"Duplicate alias '{duplicateAlias}' added to command '{this.Name}'. " +
                    $"Duplicates: '{GetArgNodeName(argumentNode)}' & '{GetArgNodeName(duplicatedArg!)}'");

                static string GetArgNodeName(IArgumentNode arg) => $"{arg.GetType().Name}:{arg.Name}(source:{arg.DefinitionSource})";
            }
        }

        argumentNode.Aliases.ForEach(a => _argumentsByAlias.Add(a, argumentNode));
    }

    public override string ToString() => $"Command {nameof(Command)}:{Name}  ({DefinitionSource})";

    public string ToString(bool verbose) =>
        !verbose
            ? $"Command {nameof(Command)}:{Name} ({DefinitionSource})"
            : $"Command {nameof(Command)}:{Name} ({DefinitionSource})" +
              $"operands:{_operands.Select(o => o.Name).ToCsv()} " +
              $"options:{_options.Select(o => o.Name).ToCsv()} " +
              $"commands:{_subcommands.Select(c => c.Name).ToCsv()}";

    // ReSharper disable once RedundantCast
    public static bool operator ==(Command x, Command y) => (object)x == y;

    public static bool operator !=(Command x, Command y) => !(x == y);

    private bool Equals(Command other) =>
        string.Equals(Name, other.Name) && Equals(Parent, other.Parent);

    public override bool Equals(object? obj) =>
        obj is not null
        && (ReferenceEquals(this, obj)
            || obj.GetType() == GetType()
            && Equals((Command) obj));

    public override int GetHashCode()
    {
        unchecked
        {
            return (Name.GetHashCode() * 397) ^ (Parent is not null ? Parent.GetHashCode() : 0);
        }
    }
}