using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandDotNet.Diagnostics.Parse;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Tokens;

namespace CommandDotNet.Diagnostics
{
    internal static class ParseDirective
    {
        internal static AppRunner UseParseDirective(this AppRunner appRunner)
        {
            return appRunner.Configure(c =>
            {
                c.UseMiddleware(ConfigureParseReportByTokenTransform, MiddlewareStages.PreTokenize);
                c.UseMiddleware(ParseReportByArg, MiddlewareStages.BindValues, MiddlewareSteps.BindValues.Order + 100);
            });
        }

        // adapted from https://github.com/dotnet/command-line-api directives
        private static Task<int> ConfigureParseReportByTokenTransform(CommandContext commandContext, ExecutionDelegate next)
        {
            if (!commandContext.Tokens.TryGetDirective("parse", out string value))
            {
                return next(commandContext);
            }

            var parseContext = ParseContext.Parse(value);
            commandContext.Services.AddOrUpdate(parseContext);
            CaptureTransformations(commandContext, parseContext);

            Action<string> writeln = commandContext.Console.Out.WriteLine;

            try
            {
                // ParseReportByArg is run within this pipeline
                var result = next(commandContext);
                if (!parseContext.Reported)
                {
                    // in case ParseReportByArg wasn't run due to parsing errors,
                    // output this the transformations as a temporary aid
                    writeln(null);
                    writeln(commandContext.ParseResult.HelpWasRequested() 
                        ? "Help requested. Only token transformations are available."
                        : "Unable to map tokens to arguments. Falling back to token transformations.");
                    writeln(parseContext.Transformations.ToString());
                }
                else if (parseContext.IncludeTokenization)
                {
                    writeln(parseContext.Transformations.ToString());
                }
                else
                {
                    writeln(null);
                    writeln($"Use [parse:{ParseContext.IncludeTransformationsArgName}]" +
                            " to include token transformations.");
                }

return result;
            }
            catch (Exception) when (!parseContext.Reported)
            {
                // in case ParseReportByArg wasn't run due to parsing errors,
                // output this the transformations as a temporary aid
                writeln(null);
                writeln("Unable to map tokens to arguments. Falling back to token transformations.");
                writeln(parseContext.Transformations.ToString());
                throw;
            }
        }

        private static Task<int> ParseReportByArg(CommandContext commandContext, ExecutionDelegate next)
        {
            var parseContext = commandContext.Services.Get<ParseContext>();
            if (parseContext != null)
            {
                ParseReporter.Report(
                    commandContext, 
                    commandContext.Console.Out.WriteLine);
                parseContext.Reported = true;
                return Task.FromResult(0);
            }

            return next(commandContext);
        }

        private static void CaptureTransformations(CommandContext commandContext, ParseContext parseContext)
        {
            void WriteLine(string ln) => parseContext.Transformations.AppendLine(ln);

            WriteLine(null);
            WriteLine("token transformations:");
            WriteLine(null);

            CaptureTransformation(WriteLine, commandContext.Tokens, ">>> from shell");

            commandContext.AppConfig.TokenizationEvents.OnTokenTransformation += args =>
            {
                if (args.PreTransformTokens.Count == args.PostTransformTokens.Count &&
                    Enumerable.Range(0, args.PreTransformTokens.Count)
                        .All(i => args.PreTransformTokens[i] == args.PostTransformTokens[i]))
                {
                    CaptureTransformation(WriteLine, null, $">>> after: {args.Transformation.Name} (no changes)");
                }
                else
                {
                    CaptureTransformation(WriteLine, args.PostTransformTokens,
                        $">>> after: {args.Transformation.Name}");
                }
            };
        }

        private static void CaptureTransformation(Action<string> writeLine, TokenCollection args, string description)
        {
            writeLine(description);

            if (args != null)
            {
                var maxTokenTypeNameLength = Enum.GetNames(typeof(TokenType)).Max(n => n.Length);

                foreach (var arg in args)
                {
                    var outputFormat = $"  {{0, -{maxTokenTypeNameLength}}}: {{1}}";
                    writeLine(string.Format(outputFormat, arg.TokenType, arg.RawValue));
                }
            }
        }

        private class ParseContext
        {
            internal const string IncludeTransformationsArgName = "t";

            internal bool IncludeTokenization;
            internal bool Reported;
            internal readonly StringBuilder Transformations = new StringBuilder();

            internal static ParseContext Parse(string value)
            {
                var parts = value.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length <= 1)
                {
                    return new ParseContext();
                }

                var settings = parts[1].Split(';')
                    .Select(p => p.Split('='))
                    .ToDictionary(p => p[0], p => p.Length > 1 ? p[1] : null, StringComparer.OrdinalIgnoreCase);

                return new ParseContext
                {
                    IncludeTokenization = settings.ContainsKey(IncludeTransformationsArgName)
                };
            }
        }
    }
}