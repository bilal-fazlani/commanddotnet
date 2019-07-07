using System;
using System.Collections.Generic;

namespace CommandDotNet.Parsing
{
    public class ParserContext
    {
        private readonly ExecutionResult _executionResult;

        public event Action<(
            ExecutionResult executionResult, 
            InputTransformation transformation, 
            TokenCollection pre, 
            TokenCollection post)> OnInputTransformation;

        public event Action<ExecutionResult> OnTokenizationCompleted;

        public ParserContext(ExecutionResult executionResult)
        {
            _executionResult = executionResult;
        }

        internal IEnumerable<InputTransformation> InputTransformations { get; set; }

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