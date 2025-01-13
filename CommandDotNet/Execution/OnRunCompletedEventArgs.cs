using System;

namespace CommandDotNet.Execution;

public class OnRunCompletedEventArgs(CommandContext commandContext) : EventArgs
{
    public CommandContext CommandContext { get; } = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
}