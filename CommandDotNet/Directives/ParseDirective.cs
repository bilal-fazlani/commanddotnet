using System;
using System.IO;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.Directives
{
    internal class ParseDirective
    {
        // adapted from https://github.com/dotnet/command-line-api directives
        public static int Execute(ExecutionResult executionResult, Func<ExecutionResult, int> next)
        {
            if (executionResult.Tokens.TryGetDirective("parse", out string value))
            {
                var parserContext = executionResult.ParserConfig;
                var consoleOut = executionResult.AppSettings.Out;

                var parts = value.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var verbose = parts.Length > 1 && parts[1].Equals("verbose", StringComparison.OrdinalIgnoreCase);

                if (!verbose)
                {
                    consoleOut.WriteLine("use [parse:verbose] to see results after each transformation");
                }

                ReportTransformation(consoleOut, executionResult.Tokens, ">>> from shell");

                if (verbose)
                {
                    parserContext.Events.OnInputTransformation += tuple =>
                    {
                        if (tuple.pre.Count == tuple.post.Count &&
                            Enumerable.Range(0, tuple.pre.Count).All(i => tuple.pre[i] == tuple.post[i]))
                        {
                            ReportTransformation(consoleOut, null, $">>> no changes after: {tuple.transformation.Name}");
                        }
                        else
                        {
                            ReportTransformation(consoleOut, tuple.post, $">>> transformed after: {tuple.transformation.Name}");
                        }
                    };
                }
                else
                {
                    parserContext.Events.OnTokenizationCompleted += result =>
                    {
                        var transformations = parserContext.InputTransformations.Select(t => t.Name).ToCsv(" > ");
                        ReportTransformation(consoleOut, result.Tokens, $">>> transformed after: {transformations}");
                    };
                }

                parserContext.Events.OnTokenizationCompleted += result => result.ShouldExitWithCode(0);
            }

            return next(executionResult);
        }

        private static void ReportTransformation(TextWriter consoleOut, TokenCollection args, string description)
        {
            consoleOut.WriteLine(description);

            if (args != null)
            {
                var maxTokenTypeNameLength = Enum.GetNames(typeof(TokenType)).Max(n => n.Length);

                foreach (var arg in args)
                {
                    var outputFormat = $"  {{0, -{maxTokenTypeNameLength}}}: {{1}}";
                    consoleOut.WriteLine(outputFormat, arg.TokenType, arg.RawValue);
                }
            }

            consoleOut.WriteLine();
        }
    }
}