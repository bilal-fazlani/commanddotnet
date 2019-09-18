using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandDotNet.Execution
{
    internal static class ExecutionMiddlewareExtensions
    {
        internal static Task<int> InvokePipeline(this IEnumerable<ExecutionMiddleware> pipeline, CommandContext commandContext)
        {
            var middlewareChain = pipeline.Aggregate(
                (first, second) =>
                    (ctx, next) =>
                        first(ctx, c => c.AppConfig.CancellationToken.IsCancellationRequested
                            ? Task.FromResult(0)
                            : second(c, next)));

            return middlewareChain(commandContext, ctx => Task.FromResult(0));
        }
    }
}