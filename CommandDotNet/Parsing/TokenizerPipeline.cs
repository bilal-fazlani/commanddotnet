using System;
using System.Linq;

namespace CommandDotNet.Parsing
{
    internal class TokenizerPipeline
    {
        private readonly ParserContext _parserContext;
            
        public TokenizerPipeline(ParserContext parserContext)
        {
            _parserContext = parserContext ?? throw new ArgumentNullException(nameof(parserContext));
        }

        public void Tokenize(ExecutionResult executionResult)
        {
            InsertSystemTransformations();
            executionResult.Tokens = ApplyInputTransformations(executionResult.OriginalTokens);
            _parserContext.TokenizationCompleted();
        }

        private TokenCollection ApplyInputTransformations(TokenCollection args)
        {
            _parserContext.InputTransformations = _parserContext.InputTransformations.OrderBy(t => t.Order).AsEnumerable();
            foreach (var transformation in _parserContext.InputTransformations)
            {
                try
                {
                    var tempArgs = transformation.Transformation(args);
                    _parserContext.InputTransformation(transformation, args, tempArgs);
                    args = tempArgs;
                }
                catch (Exception e)
                {
                    throw new AppRunnerException($"transformation failure for: {transformation}", e);
                }
            }

            return args;
        }

        private void InsertSystemTransformations()
        {
            // append system transformations to the end.
            // these are features we want to ensure is applied to all arguments
            // parsing logic depends on these being processed
            _parserContext.InputTransformations = _parserContext.InputTransformations.Union(
                new[]
                {
                    new InputTransformation(
                        "expand-clubbed-flags",
                        Int32.MaxValue,
                        Tokenizer.ExpandClubbedOptions),
                    new InputTransformation(
                        "split-option-assignments",
                        Int32.MaxValue,
                        Tokenizer.SplitOptionAssignments)
                });
        }
    }
}