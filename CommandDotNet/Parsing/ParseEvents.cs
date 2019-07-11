using System;
using CommandDotNet.Execution;

namespace CommandDotNet.Parsing
{
    public class ParseEvents
    {
        public event Action<(
            CommandContext executionResult,
            InputTransformation transformation,
            TokenCollection pre,
            TokenCollection post)> OnInputTransformation;

        public event Action<CommandContext> OnTokenizationCompleted;

        internal void InputTransformation(CommandContext commandContext, InputTransformation transformation, TokenCollection pre, TokenCollection post)
        {
            OnInputTransformation?.Invoke((commandContext, transformation, pre, post));
        }

        internal void TokenizationCompleted(CommandContext commandContext)
        {
            OnTokenizationCompleted?.Invoke(commandContext);
        }
    }
}