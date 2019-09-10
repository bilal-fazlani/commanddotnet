using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

namespace CommandDotNet.Directives
{
    internal static class ParseDirective
    {
        internal static AppRunner UseParseDirective(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(Report, MiddlewareStages.PreTransformTokens);
                c.UseMiddleware(ExitAfterReport, MiddlewareStages.TransformTokens, int.MaxValue);
            });
        }

        // adapted from https://github.com/dotnet/command-line-api directives
        private static Task<int> Report(CommandContext commandContext, ExecutionDelegate next)
        {
            if (commandContext.Tokens.TryGetDirective("parse", out string value))
            {
                var appConfig = commandContext.AppConfig;
                var console = commandContext.Console;

                var parts = value.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var verbose = parts.Length > 1 && parts[1].Equals("verbose", StringComparison.OrdinalIgnoreCase);

                if (!verbose)
                {
                    console.Out.WriteLine("use [parse:verbose] to see results after each transformation");
                }

                ReportTransformation(console, commandContext.Tokens, ">>> from shell");

                if (verbose)
                {
                    appConfig.TokenizationEvents.OnTokenTransformation += args =>
                    {
                        if (args.PreTransformTokens.Count == args.PostTransformTokens.Count &&
                            Enumerable.Range(0, args.PreTransformTokens.Count).All(i => args.PreTransformTokens[i] == args.PostTransformTokens[i]))
                        {
                            ReportTransformation(console, null, $">>> no changes after: {args.Transformation.Name}");
                        }
                        else
                        {
                            ReportTransformation(console, args.PostTransformTokens, $">>> transformed after: {args.Transformation.Name}");
                        }
                    };
                }
                else
                {
                    appConfig.TokenizationEvents.OnTokenizationCompleted += args =>
                    {
                        var transformations = appConfig.TokenTransformations.Select(t => t.Name).ToCsv(" > ");
                        ReportTransformation(console, args.CommandContext.Tokens, $">>> transformed after: {transformations}");
                    };
                }
            }

            return next(commandContext);
        }

        private static Task<int> ExitAfterReport(CommandContext commandContext, ExecutionDelegate next)
        {
            return commandContext.Tokens.TryGetDirective("parse", out _)
                ? Task.FromResult(0)
                : next(commandContext);
        }

        private static void ReportTransformation(IConsole consoleOut, TokenCollection args, string description)
        {
            consoleOut.Out.WriteLine(description);

            if (args != null)
            {
                var maxTokenTypeNameLength = Enum.GetNames(typeof(TokenType)).Max(n => n.Length);

                foreach (var arg in args)
                {
                    var outputFormat = $"  {{0, -{maxTokenTypeNameLength}}}: {{1}}";
                    consoleOut.Out.WriteLine(string.Format(outputFormat, arg.TokenType, arg.RawValue));
                }
            }
        }
    }
}