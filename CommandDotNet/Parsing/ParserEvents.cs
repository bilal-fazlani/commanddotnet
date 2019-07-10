using System;
using CommandDotNet.Execution;

namespace CommandDotNet.Parsing
{
    public class ParserEvents
    {
        private readonly CommandContext _commandContext;


        public event Action<(
            CommandContext executionResult,
            InputTransformation transformation,
            TokenCollection pre,
            TokenCollection post)> OnInputTransformation;

        public event Action<CommandContext> OnTokenizationCompleted;

        public ParserEvents(CommandContext commandContext)
        {
            _commandContext = commandContext;
        }

        internal void InputTransformation(InputTransformation transformation, TokenCollection pre, TokenCollection post)
        {
            OnInputTransformation?.Invoke((_commandContext, transformation, pre, post));
        }

        internal void TokenizationCompleted()
        {
            OnTokenizationCompleted?.Invoke(_commandContext);
        }
    }
}