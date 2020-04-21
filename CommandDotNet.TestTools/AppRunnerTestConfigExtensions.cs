using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools
{
    public static class AppRunnerTestConfigExtensions
    {
        public static AppRunner InjectTrackingInvocations(this AppRunner runner)
        {
            return runner.Configure(c => c.UseMiddleware(TrackingInvocationMiddleware, 
                MiddlewareStages.PostBindValuesPreInvoke, 
                int.MaxValue));
        }

        internal static Task<int> TrackingInvocationMiddleware(CommandContext context, ExecutionDelegate next)
        {
            context.InvocationPipeline.All.ForEach(step => step.Invocation = new TrackingInvocation(step.Invocation));
            return next(context);
        }
    }
}