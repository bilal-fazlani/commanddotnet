using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Directives;
using CommandDotNet.Execution;

namespace CommandDotNet.Localization
{
    internal static class CultureDirective
    {
        internal static AppRunner UseCultureDirective(AppRunner appRunner)
        {
            return appRunner.Configure(c => c.UseMiddleware(CheckCulture, MiddlewareSteps.DebugDirective + 1));
        }

        private static Task<int> CheckCulture(CommandContext context, ExecutionDelegate next)
        {
            Action? revert = null;

            if (context.Tokens.TryGetDirective("culture", out string? culture))
            {
                var name = culture!.Split(':').Last();
                var cultureInfo = CultureInfo.GetCultureInfo(name);

                var previousCulture = CultureInfo.CurrentCulture;
                var previousUICulture = CultureInfo.CurrentUICulture;

                revert = () =>
                {
                    CultureInfo.CurrentCulture = previousCulture;
                    CultureInfo.CurrentUICulture = previousUICulture;
                };

                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
            }

            var result = next(context);

            // revert for tests and interactive repl sessions
            revert?.Invoke();

            return result;
        }
    }
}