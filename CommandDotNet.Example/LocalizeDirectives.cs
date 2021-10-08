﻿using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Directives;
using CommandDotNet.Execution;

namespace CommandDotNet.Example
{
    public static class LocalizeDirectives
    {
        public static AppRunner UseLocalizeDirective(this AppRunner appRunner)
        {
            return appRunner.Configure(c => c.UseMiddleware(Localize, MiddlewareSteps.DebugDirective + 1));
        }

        private static Task<int> Localize(CommandContext context, ExecutionDelegate next)
        {
            if (context.Tokens.TryGetDirective("loc", out string? culture))
            {
                var name = culture!.Split(':').Last();
                var cultureInfo = CultureInfo.GetCultureInfo(name);
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
            }

            return next(context);
        }
    }
}