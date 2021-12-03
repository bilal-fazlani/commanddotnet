using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Rendering;

namespace CommandDotNet.TestTools
{
    public static class AppRunnerTestConfigExtensions
    {
        public static AppRunner TrackingInvocations(this AppRunner runner)
        {
            return runner.Configure(c => c.UseMiddleware(TrackingInvocationMiddleware, 
                new MiddlewareStep(MiddlewareStages.PostBindValuesPreInvoke, short.MaxValue)));
        }

        internal static Task<int> TrackingInvocationMiddleware(CommandContext context, ExecutionDelegate next)
        {
            context.InvocationPipeline.All.ForEach(step => step.Invocation = new TrackingInvocation(step.Invocation));
            return next(context);
        }
        
        /// <summary>
        /// Intercepts <see cref="Console.Out"/> and <see cref="Console.Error"/>.
        /// Text is still written to those writers, but also to IConsole.<br/>
        /// BEWARE: this is not suitable for tests run in parallel.<br/>
        /// </summary>
        /// <param name="runner">the <see cref="AppRunner"/></param>
        /// <param name="middlewareStep">
        /// By default, it only wraps the Invocation of the command and interceptor methods.<br/>
        /// To intercept sooner, provide a step for earlier in the pipeline.
        /// </param>
        public static AppRunner InterceptSystemConsoleWrites(this AppRunner runner, MiddlewareStep? middlewareStep = null)
        {
            return runner.Configure(c => c.UseMiddleware(InterceptingSystemConsoleWrites,
                middlewareStep ?? new MiddlewareStep(MiddlewareStages.Invoke, short.MinValue)));
        }

        private static Task<int> InterceptingSystemConsoleWrites(CommandContext context, ExecutionDelegate next)
        {
            if (Console.Out is DuplexTextWriter)
            {
                throw new InvalidOperationException("System.Console is already being intercepted. Interception does not currently support parellel executions.");
            }
            
            var @out = new DuplexTextWriter(Console.Out, context.Console.Out);
            var error = new DuplexTextWriter(Console.Error, context.Console.Error);
            try
            {
                Console.SetOut(@out);
                Console.SetError(error);
                return next(context);
            }
            finally
            {
                Console.SetOut(@out.Original);
                Console.SetError(error.Original);
            }
        }
    }
}