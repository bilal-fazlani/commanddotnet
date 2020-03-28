using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Directives.Parse;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.CommandLogger
{
    public static class CommandLoggerMiddleware
    {
        public static AppRunner UseCommandLogger(this AppRunner appRunner,
            Func<CommandContext, Action<string>> writerFactory = null,
            bool includeSystemInfo = false,
            bool includeAppConfig = false,
            Func<CommandContext, IEnumerable<(string key, string value)>> additionalInfoCallback = null)
        {
            return appRunner.Configure(c =>
            {
                c.Services.Add(new LogConfig(
                    writerFactory,
                    includeSystemInfo,
                    includeAppConfig,
                    additionalInfoCallback));
                c.UseMiddleware(CommandLogger, MiddlewareStages.PostBindValuesPreInvoke);
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
                WriterFactory = writerFactory ?? (ctx => ctx.Console.Out.WriteLine);
                IncludeSystemInfo = includeSystemInfo;
                IncludeAppConfig = includeAppConfig;
                AdditionalHeadersCallback = additionalHeadersCallback;
            }
        }

        private static Task<int> CommandLogger(CommandContext commandContext, ExecutionDelegate next)
        {
            var config = commandContext.AppConfig.Services.Get<LogConfig>();
            var writer = config.WriterFactory(commandContext);
            if (writer != null)
            {
                var header = BuildHeader(commandContext, config);
                writer(header);
            }
            return next(commandContext);
        }

        private static string BuildHeader(CommandContext commandContext, LogConfig config)
        {
            var sb = new StringBuilder(Environment.NewLine);

            sb.AppendLine("***************************************");

            var originalArgs = RemovePasswords(commandContext, commandContext.Original.Args.ToCsv(" "));
            sb.AppendLine("Original input:");
            sb.AppendLine($"  {originalArgs}");
            sb.AppendLine();

            var indent = new Indent();
            ParseReporter.Report(commandContext, s => sb.AppendLine(s), indent);

            var otherConfigEntries = GetOtherConfigInfo(commandContext, config).ToList();
            if (!otherConfigEntries.IsNullOrEmpty())
            {
                sb.AppendLine();
                var maxName = otherConfigEntries.Max(e => e.name.Length);
                foreach (var entry in otherConfigEntries)
                {
                    sb.AppendFormat($"{{0, -{maxName + 1}}} = {{1}}", entry.name, entry.text);
                    sb.AppendLine();
                }
            }

            if (config.IncludeAppConfig)
            {
                sb.AppendLine();
                sb.AppendLine(commandContext.AppConfig.ToString("  ", 1));
            }

            sb.AppendLine("***************************************");

            return sb.ToString();
        }

        private static string RemovePasswords(CommandContext commandContext, string originalArgs)
        {
            commandContext.ParseResult.TargetCommand.AllArguments(includeInterceptorOptions: true)
                .Where(a => a.IsObscured())
                .ForEach(a => a.InputValues
                    .Where(iv => iv.Source == Constants.InputValueSources.Argument)
                    .SelectMany(iv => iv.Values)
                    .ForEach(pwd => originalArgs = originalArgs.Replace(pwd, Password.ValueReplacement)));
            return originalArgs;
        }

        private static IEnumerable<(string name, string text)> GetOtherConfigInfo(CommandContext commandContext, LogConfig config)
        {
            if (config.IncludeSystemInfo)
            {
                var appInfo = AppInfo.GetAppInfo(commandContext);
                yield return ("Tool version", $"{appInfo.FileName} {appInfo.Version}");
                yield return (".Net version", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.Trim());
                yield return ("OS version", System.Runtime.InteropServices.RuntimeInformation.OSDescription.Trim());
                yield return ("Machine", Environment.MachineName);
                yield return ("Username", $"{Environment.UserDomainName}\\{Environment.UserName}");
            }

            var info = config.AdditionalHeadersCallback?.Invoke(commandContext);
            if (info != null)
            {
                foreach (var header in info)
                {
                    yield return (header.key, header.value);
                }
            }
        }
    }
}
