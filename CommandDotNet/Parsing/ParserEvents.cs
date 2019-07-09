using System;
using CommandDotNet.Execution;

namespace CommandDotNet.Parsing
{
    public class ParserEvents
    {
        private readonly ExecutionContext _executionContext;


        public event Action<(
            ExecutionContext executionResult,
            InputTransformation transformation,
            TokenCollection pre,
            TokenCollection post)> OnInputTransformation;

        public event Action<ExecutionContext> OnTokenizationCompleted;

        public ParserEvents(ExecutionContext executionContext)
        {
            _executionContext = executionContext;
        }

        internal void InputTransformation(InputTransformation transformation, TokenCollection pre, TokenCollection post)
        {
            OnInputTransformation?.Invoke((_executionContext, transformation, pre, post));
        }

        internal void TokenizationCompleted()
        {
            OnTokenizationCompleted?.Invoke(_executionContext);
        }
    }
}