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
            commandContext.Tokens = ApplyInputTransformations(commandContext);
            commandContext.ExecutionConfig.ParseEvents.TokenizationCompleted(commandContext);

            return next(commandContext);
        }

        private static TokenCollection ApplyInputTransformations(CommandContext commandContext)
        {
            var tokens = commandContext.Tokens;
            var executionConfig = commandContext.ExecutionConfig;
            foreach (var transformation in executionConfig.InputTransformations)
            {
                try
                {
                    var tempArgs = transformation.Transformation(tokens);
                    executionConfig.ParseEvents.InputTransformation(commandContext, transformation, tokens, tempArgs);
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
            // append system transformations to the end.
            // these are features we want to ensure are applied to all arguments
            // because parsing logic depends on these being processed
            executionConfig.InputTransformations = executionConfig.InputTransformations
                .OrderBy(t => t.Order)
                .Union(
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
                    })
                .ToArray();
        }
    }
}