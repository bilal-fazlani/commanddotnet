using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Parsing
{
    internal static class TokenizerPipeline
    {
        public static Task<int> TokenizeMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            InsertSystemTransformations(commandContext.ExecutionConfig);
            commandContext.Tokens = ApplyTokenTransformations(commandContext);
            commandContext.ExecutionConfig.ParseEvents.TokenizationCompleted(commandContext);

            return next(commandContext);
        }

        private static TokenCollection ApplyTokenTransformations(CommandContext commandContext)
        {
            var tokens = commandContext.Tokens;
            var executionConfig = commandContext.ExecutionConfig;
            foreach (var transformation in executionConfig.TokenTransformations)
            {
                try
                {
                    var tempArgs = transformation.Transformation(tokens);
                    executionConfig.ParseEvents.TokenTransformation(commandContext, transformation, tokens, tempArgs);
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
            executionConfig.TokenTransformations = executionConfig.TokenTransformations
                .OrderBy(t => t.Order)
                .Union(
                    new[]
                    {
                        new TokenTransformation(
                            "expand-clubbed-flags",
                            Int32.MaxValue,
                            Tokenizer.ExpandClubbedOptions),
                        new TokenTransformation(
                            "split-option-assignments",
                            Int32.MaxValue,
                            Tokenizer.SplitOptionAssignments)
                    })
                .ToArray();
        }
    }
}