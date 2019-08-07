using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Tokens
{
    internal static class TokenizerPipeline
    {
        public static Task<int> TokenizeMiddleware(CommandContext commandContext, Func<CommandContext, Task<int>> next)
        {
            InsertSystemTransformations(commandContext.AppConfig);
            commandContext.Tokens = ApplyTokenTransformations(commandContext);
            commandContext.AppConfig.ParseEvents.TokenizationCompleted(commandContext);

            return next(commandContext);
        }

        private static TokenCollection ApplyTokenTransformations(CommandContext commandContext)
        {
            var tokens = commandContext.Tokens;
            var appConfig = commandContext.AppConfig;
            foreach (var transformation in appConfig.TokenTransformations)
            {
                try
                {
                    var tempArgs = transformation.Transformation(tokens);
                    appConfig.ParseEvents.TokenTransformation(commandContext, transformation, tokens, tempArgs);
                    tokens = tempArgs;
                }
                catch (Exception e)
                {
                    throw new AppRunnerException($"transformation failure for: {transformation}", e);
                }
            }

            return tokens;
        }

        private static void InsertSystemTransformations(AppConfig appConfig)
        {
            // append system transformations to the end.
            // these are features we want to ensure are applied to all arguments
            // because parsing logic depends on these being processed
            appConfig.TokenTransformations = appConfig.TokenTransformations
                .OrderBy(t => t.Order)
                .Union(
                    new[]
                    {
                        new TokenTransformation(
                            "expand-clubbed-flags",
                            Int32.MaxValue,
                            ExpandClubbedOptionsTransformation.Transform),
                        new TokenTransformation(
                            "split-option-assignments",
                            Int32.MaxValue,
                            SplitOptionsTransformation.Transform)
                    })
                .ToArray();
        }
    }
}