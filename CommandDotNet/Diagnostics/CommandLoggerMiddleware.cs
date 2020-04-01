using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Directives;
using CommandDotNet.Execution;

namespace CommandDotNet.Diagnostics
{
    internal static class CommandLoggerMiddleware
    {
        internal static AppRunner UseCommandLogger(AppRunner appRunner,
            Func<CommandContext, Action<string>> writerFactory,
            bool includeSystemInfo,
            bool includeAppConfig,
            Func<CommandContext, IEnumerable<(string key, string value)>> additionalInfoCallback)
        {
            return appRunner.Configure(c =>
            {
                c.Services.Add(new LogConfig(
                    writerFactory,
                    includeSystemInfo,
                    includeAppConfig,
                    additionalInfoCallback));
                c.UseMiddleware(CommandLogger, MiddlewareStages.Invoke);
            });
        }

        private class LogConfig
        {
            public Func<CommandContext, Action<string>> WriterFactory { get; }
            public bool IncludeSystemInfo { get; }
            public bool IncludeAppConfig { get; }
            public Func<CommandContext, IEnumerable<(string key, string value)>> AdditionalHeadersCallback { get; }

            public LogConfig(
                Func<CommandContext, Action<string>> writerFactory,
                bool includeSystemInfo,
                bool includeAppConfig,
                Func<CommandContext, IEnumerable<(string, string)>> additionalHeadersCallback)
            {
                WriterFactory = writerFactory;
                IncludeSystemInfo = includeSystemInfo;
                IncludeAppConfig = includeAppConfig;
                AdditionalHeadersCallback = additionalHeadersCallback;
            }
        }

        private static Action<string> DefaultWriterFactory(CommandContext ctx)
        {
            return ctx.Tokens.TryGetDirective("cmdlog", out _) 
                ? ctx.Console.Out.WriteLine 
                : (Action<string>)null;
        }

        private static Task<int> CommandLogger(CommandContext commandContext, ExecutionDelegate next)
        {
            var config = commandContext.AppConfig.Services.Get<LogConfig>();
            var writer = (config.WriterFactory ?? DefaultWriterFactory)(commandContext);
            if (writer != null)
            {
                Diagnostics.CommandLogger.Log(
                    commandContext, 
                    writer, 
                    config.IncludeSystemInfo, 
                    config.IncludeAppConfig, 
                    config.AdditionalHeadersCallback?.Invoke(commandContext));
            }

            return next(commandContext);
        }
    }
}
