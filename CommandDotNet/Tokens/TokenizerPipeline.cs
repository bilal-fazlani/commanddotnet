using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.Tokens
{
    internal static class TokenizerPipeline
    {
        public static Task<int> TokenizeInputMiddleware(CommandContext commandContext, ExecutionDelegate next)
        {
            InsertSystemTransformations(commandContext.AppConfig);
            commandContext.Tokens = ApplyTokenTransformations(commandContext);
            commandContext.AppConfig.TokenizationEvents.TokenizationCompleted(commandContext);

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
                    var tempArgs = transformation.Transformation(commandContext, tokens);
                    tempArgs.Where(t => t.SourceName.IsNullOrWhitespace()).ForEach(t => t.SourceName = transformation.Name);
                    appConfig.TokenizationEvents.TokenTransformation(commandContext, transformation, tokens, tempArgs);
                    tokens = tempArgs;
                }
                catch (Exception e)
                {
                    throw new TokenTransformationException(transformation, e);
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