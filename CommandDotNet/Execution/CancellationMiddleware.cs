using System.Threading.Tasks;

namespace CommandDotNet.Execution
{
    internal static class CancellationMiddleware
    {
        internal static AppRunner UseCancellationHandlers(AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(AddCancellationTokens, MiddlewareSteps.CancellationHandler);
                c.OnRunCompleted += args => CancellationHandlers.EndRun(args.CommandContext);
            });
        }

        private static Task<int> AddCancellationTokens(CommandContext ctx, ExecutionDelegate next)
        {
            CancellationHandlers.BeginRun(ctx);
            return next(ctx);
        }
    }
}