using System;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;
using CommandDotNet.Rendering;

namespace CommandDotNet.Directives
{
    internal static class ParseDirective
    {
        // adapted from https://github.com/dotnet/command-line-api directives
        public static int ParseMiddleware(CommandContext commandContext, Func<CommandContext, int> next)
        {
            if (commandContext.Tokens.TryGetDirective("parse", out string value))
            {
                var executionConfig = commandContext.ExecutionConfig;
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
                    executionConfig.Events.OnInputTransformation += tuple =>
                    {
                        if (tuple.pre.Count == tuple.post.Count &&
                            Enumerable.Range(0, tuple.pre.Count).All(i => tuple.pre[i] == tuple.post[i]))
                        {
                            ReportTransformation(console, null, $">>> no changes after: {tuple.transformation.Name}");
                        }
                        else
                        {
                            ReportTransformation(console, tuple.post, $">>> transformed after: {tuple.transformation.Name}");
                        }
                    };
                }
                else
                {
                    executionConfig.Events.OnTokenizationCompleted += ctx =>
                    {
                        var transformations = executionConfig.InputTransformations.Select(t => t.Name).ToCsv(" > ");
                        ReportTransformation(console, ctx.Tokens, $">>> transformed after: {transformations}");
                    };
                }

                executionConfig.Events.OnTokenizationCompleted += ctx => ctx.ShouldExitWithCode(0);
            }

            return next(commandContext);
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