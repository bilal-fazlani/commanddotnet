using System.Diagnostics;
using System.Threading.Tasks;
using CommandDotNet.Directives;
using CommandDotNet.Execution;

namespace CommandDotNet.Diagnostics
{
    internal static class TimeDirective
    {
        internal static AppRunner UseTimeDirective(this AppRunner appRunner)
        {
            return appRunner.Configure(c => c.UseMiddleware(TimeCommand, MiddlewareSteps.DebugDirective + 1));
        }

        private static Task<int> TimeCommand(CommandContext context, ExecutionDelegate next)
        {
            if (context.Original.Tokens.TryGetDirective(Resources.A.Time_time, out _))
            {
                var sw = Stopwatch.StartNew();
                var result = next(context);
                sw.Stop();
                context.Console.WriteLine();
                context.Console.WriteLine($"{Resources.A.Time_time}: {sw.Elapsed}");
                return result;
            }

            return next(context);
        }
    }
}