using System;
using System.Linq;
using CommandDotNet.Execution;

namespace CommandDotNet.Parsing
{
    internal static class TokenizerPipeline
    {
        public static int TokenizeMiddleware(CommandContext commandContext, Func<CommandContext, int> next)
        {
            InsertSystemTransformations(commandContext.ExecutionConfig);
            commandContext.Tokens = ApplyInputTransformations(commandContext.Tokens, commandContext.ExecutionConfig);
            commandContext.ExecutionConfig.Events.TokenizationCompleted();

            return next(commandContext);
        }

        private static TokenCollection ApplyInputTransformations(TokenCollection tokens, ExecutionConfig executionConfig)
        {
            foreach (var transformation in executionConfig.InputTransformations)
            {
                try
                {
                    var tempArgs = transformation.Transformation(tokens);
                    executionConfig.Events.InputTransformation(transformation, tokens, tempArgs);
                    tokens = tempArgs;
                }
                catch (Exception e)
                {
                    throw new AppRunnerException($"transformation failure for: {transformation}", e);
                }
            }

            return tokens;
        }

        private static void InsertSystemTransformations(ExecutionConfig executionConfig)
        {
            executionConfig.InputTransformations = executionConfig.InputTransformations.OrderBy(t => t.Order).AsEnumerable();
            // append system transformations to the end.
            // these are features we want to ensure are applied to all arguments
            // because parsing logic depends on these being processed
            executionConfig.InputTransformations = executionConfig.InputTransformations.Union(
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