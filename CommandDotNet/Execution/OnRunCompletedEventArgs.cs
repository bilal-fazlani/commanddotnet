using System;

namespace CommandDotNet.Execution
{
    public class OnRunCompletedEventArgs : EventArgs
    {
        public CommandContext CommandContext { get; }

        public OnRunCompletedEventArgs(CommandContext commandContext)
        {
            CommandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
        }
    }
}