using System;
using CommandDotNet.Execution;

namespace CommandDotNet.Builders
{
    public class BuildEvents
    {
        public event Action<CommandCreatedEventArgs> OnCommandCreated;

        internal void CommandCreated(CommandContext commandContext, ICommandBuilder commandBuilder)
        {
            OnCommandCreated?.Invoke(new CommandCreatedEventArgs(commandContext, commandBuilder));
        }

        public class CommandCreatedEventArgs : EventArgs
        {
            public CommandContext CommandContext { get; }
            public ICommandBuilder CommandBuilder { get; }

            public CommandCreatedEventArgs(CommandContext commandContext, ICommandBuilder commandBuilder)
            {
                CommandContext = commandContext;
                CommandBuilder = commandBuilder;
            }
        }
    }
}