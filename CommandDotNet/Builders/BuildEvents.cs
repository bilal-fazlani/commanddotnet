using System;
using JetBrains.Annotations;

namespace CommandDotNet.Builders;

/// <summary>
/// Subscribe to <see cref="OnCommandCreated"/> to enhance <see cref="Command"/>s on creation.<br/>
/// Set in <see cref="AppRunner.Configure"/>
/// </summary>
public class BuildEvents
{
    /// <summary>
    /// Emitted for a <see cref="Command"/> after it has been created.
    /// Arguments and subcommands defined in the class/method will have been added.
    /// </summary>
    public event Action<CommandCreatedEventArgs>? OnCommandCreated;

    internal void CommandCreated(CommandContext commandContext, ICommandBuilder commandBuilder)
    {
        OnCommandCreated?.Invoke(new CommandCreatedEventArgs(commandContext, commandBuilder));
    }

    [PublicAPI]
    public class CommandCreatedEventArgs(CommandContext commandContext, ICommandBuilder commandBuilder)
        : EventArgs
    {
        public CommandContext CommandContext { get; } = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
        public ICommandBuilder CommandBuilder { get; } = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
    }
}