using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandDotNet.Builders;
using CommandDotNet.Diagnostics.Parse;
using CommandDotNet.Extensions;

namespace CommandDotNet.Diagnostics
{
    public static class CommandLogger
    {
        public static void Log(
            CommandContext commandContext,
            Action<string> writer = null,
            bool includeSystemInfo = true,
            bool includeAppConfig = false,
            IEnumerable<(string, string)> additionalHeaders = null)
        {
            if (commandContext == null)
            {
                throw new ArgumentNullException(nameof(commandContext));
            }

            var sb = new StringBuilder(Environment.NewLine);

            sb.AppendLine("***************************************");

            var originalArgs = RemovePasswords(commandContext, commandContext.Original.Args.ToCsv(" "));
            sb.AppendLine("Original input:");
            sb.AppendLine($"  {originalArgs}");
            sb.AppendLine();

            var indent = new Indent();
            ParseReporter.Report(commandContext, s => sb.AppendLine(s), indent);

            var otherConfigEntries = GetOtherConfigInfo(commandContext, includeSystemInfo, additionalHeaders).ToList();
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

            if (includeAppConfig)
            {
                sb.AppendLine();
                sb.AppendLine(commandContext.AppConfig.ToString(indent.Increment()));
            }

            sb.AppendLine("***************************************");

            writer = writer ?? commandContext.Console.Out.WriteLine;
            writer(sb.ToString());
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

        private static IEnumerable<(string name, string text)> GetOtherConfigInfo(
            CommandContext commandContext,
            bool includeSystemInfo,
            IEnumerable<(string key, string value)> additionalHeaders)
        {
            if (includeSystemInfo)
            {
                var appInfo = AppInfo.GetAppInfo(commandContext);
                yield return ("Tool version", $"{appInfo.FileName} {appInfo.Version}");
                yield return (".Net version",
                    System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.Trim());
                yield return ("OS version", System.Runtime.InteropServices.RuntimeInformation.OSDescription.Trim());
                yield return ("Machine", Environment.MachineName);
                yield return ("Username", $"{Environment.UserDomainName}\\{Environment.UserName}");
            }

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    yield return (header.key, header.value);
                }
            }
        }
    }
}