using System;

namespace CommandDotNet.Parsing
{
    public class ParserEvents
    {
        private readonly ExecutionResult _executionResult;


        public event Action<(
            ExecutionResult executionResult,
            InputTransformation transformation,
            TokenCollection pre,
            TokenCollection post)> OnInputTransformation;

        public event Action<ExecutionResult> OnTokenizationCompleted;

        public ParserEvents(ExecutionResult executionResult)
        {
            _executionResult = executionResult;
        }

        internal void InputTransformation(InputTransformation transformation, TokenCollection pre, TokenCollection post)
        {
            OnInputTransformation?.Invoke((_executionResult, transformation, pre, post));
        }

        internal void TokenizationCompleted()
        {
            OnTokenizationCompleted?.Invoke(_executionResult);
        }
    }
}