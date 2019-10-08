using System.Collections.Generic;

namespace CommandDotNet.Prompts
{
    public interface IArgumentPrompter
    {
        ICollection<string> PromptForArgumentValues(CommandContext commandContext, IArgument argument, out bool isCancellationRequested);
    }
}