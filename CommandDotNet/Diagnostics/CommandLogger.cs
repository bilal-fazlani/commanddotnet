﻿using System;
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
        public static bool HasLoggedFor(CommandContext context)
        {
            return context.Services.GetOrDefault<CommandLoggerHasLoggedMarker>() != null;
        }

        public static void Log(
            CommandContext context,
            Action<string?>? writer = null,
            bool includeSystemInfo = true,
            bool includeAppConfig = false)
        {
            var config = context.AppConfig.Services.GetOrDefault<CommandLoggerConfig>();
            if (config == null)
            {
                throw new InvalidConfigurationException(
                    $"{nameof(CommandLoggerMiddleware)} has not been registered. " +
                    $"Try `appRunner.{nameof(AppRunnerConfigExtensions.UseCommandLogger)}()`");
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Services.AddOrUpdate(new CommandLoggerHasLoggedMarker());

            var sb = new StringBuilder(Environment.NewLine);

            sb.AppendLine("***************************************");

            var originalArgs = RemovePasswords(context, context.Original.Args.ToCsv(" "));
            sb.AppendLine(Resources.A.CommandLogger_Original_input + ":");
            sb.AppendLine($"  {originalArgs}");
            sb.AppendLine();

            var indent = new Indent();
            ParseReporter.Report(context, writeln: s => sb.AppendLine(s), indent: indent);

            var additionalHeaders = config.AdditionalHeadersCallback?.Invoke(context);
            var otherConfigEntries = GetOtherConfigInfo(context.Environment, includeSystemInfo, additionalHeaders).ToList();
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
                sb.AppendLine(context.AppConfig.ToString(indent.Increment()));
            }

            sb.Append("***************************************");
            writer ??= context.Console.Out.Write;
            writer(sb.ToString());
        }

        private static string RemovePasswords(CommandContext commandContext, string originalArgs)
        {
            commandContext.ParseResult?.TargetCommand
                .AllArguments(includeInterceptorOptions: true)
                .Where(a => a.IsObscured())
                .ForEach(a => a.InputValues
                    .Where(iv => iv.Source == Resources.A.Common_argument_lc && iv.Values is not null)
                    .SelectMany(iv => iv.Values!)
                    .ForEach(pwd => originalArgs = originalArgs.Replace(pwd, Password.ValueReplacement)));
            return originalArgs;
        }

        private static IEnumerable<(string name, string text)> GetOtherConfigInfo(IEnvironment env,
            bool includeSystemInfo,
            IEnumerable<(string key, string value)>? additionalHeaders)
        {
            if (includeSystemInfo)
            {
                var appInfo = AppInfo.Instance;
                yield return (
                    Resources.A.CommandLogger_Tool_version, 
                    $"{appInfo.FileName} {appInfo.Version}");
                yield return (
                    Resources.A.CommandLogger_DotNet_version,
                    env.FrameworkDescription);
                yield return (
                    Resources.A.CommandLogger_OS_version, 
                    env.OSDescription);
                yield return (
                    Resources.A.CommandLogger_Machine,
                    env.MachineName);
                yield return (
                    Resources.A.CommandLogger_Username, 
                    $"{env.UserDomainName}\\{env.UserName}");
            }

            if (additionalHeaders is { })
            {
                foreach (var header in additionalHeaders)
                {
                    yield return (header.key, header.value);
                }
            }
        }

        private class CommandLoggerHasLoggedMarker { }
    }
}