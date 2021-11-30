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
                c.UseMiddleware(ConfigureParseReportByTokenTransform, MiddlewareSteps.ParseDirective);
                c.UseMiddleware(ParseReportByArg, MiddlewareSteps.BindValues + 100);
            });
        }

        // adapted from https://github.com/dotnet/command-line-api directives
        private static Task<int> ConfigureParseReportByTokenTransform(CommandContext commandContext, ExecutionDelegate next)
        {
            if (!commandContext.Tokens.TryGetDirective(Resources.A.ParseDirective_parse_lc, out string? value))
            {
                return next(commandContext);
            }

            var parseContext = ParseContext.Parse(value!);
            commandContext.Services.AddOrUpdate(parseContext);
            CaptureTransformations(commandContext, parseContext);

            var writer = commandContext.Console.Out;

            void WriteLine(string? line)
            {
                if (writer == null)
                {
                    throw new ArgumentNullException(nameof(writer));
                }

                // avoid extra new lines
                writer.Write(line);
                if (!line?.EndsWith(Environment.NewLine) ?? false)
                {
                    writer.Write(Environment.NewLine);
                }
            }

            try
            {
                // ParseReportByArg is run within this pipeline
                var result = next(commandContext);
                if (!parseContext.Reported)
                {
                    // in case ParseReportByArg wasn't run due to parsing errors,
                    // output this the transformations as a temporary aid
                    writer.WriteLine();
                    writer.WriteLine(commandContext.ParseResult!.HelpWasRequested()
                        ? Resources.A.ParseDirective_Help_was_requested
                        : Resources.A.ParseDirective_Unable_to_map_tokens_to_arguments);
                    WriteLine(parseContext.Transformations.ToString());
                }
                else if (parseContext.IncludeTokenization || parseContext.IncludeRawCommandLine)
                {
                    if (parseContext.IncludeTokenization)
                    {
                        WriteLine(parseContext.Transformations.ToString());
                    }
                    if (parseContext.IncludeRawCommandLine)
                    {
                        WriteLine(commandContext.Environment.CommandLine);
                    }
                }
                else
                {
                    writer.WriteLine();
                    writer.WriteLine(Resources.A.ParseDirective_Usage(
                        ParseContext.IncludeTransformationsArgName,
                        ParseContext.IncludeRawCommandLineArgName));
                }

                return result;
            }
            catch (Exception) when (!parseContext.Reported)
            {
                // in case ParseReportByArg wasn't run due to parsing errors,
                // output this the transformations as a temporary aid
                writer.WriteLine();
                writer.WriteLine(Resources.A.ParseDirective_Unable_to_map_tokens_to_arguments);
                WriteLine(parseContext.Transformations.ToString());
                throw;
            }
        }

        private static Task<int> ParseReportByArg(CommandContext commandContext, ExecutionDelegate next)
        {
            var parseContext = commandContext.Services.GetOrDefault<ParseContext>();
            if (parseContext != null)
            {
                ParseReporter.Report(
                    commandContext, 
                    includeRawCommandLine: parseContext.IncludeRawCommandLine,
                    writeln: s => commandContext.Console.Out.WriteLine(s));
                parseContext.Reported = true;
                return ExitCodes.Success;
            }

            return next(commandContext);
        }

        private static void CaptureTransformations(CommandContext commandContext, ParseContext parseContext)
        {
            void WriteLine(string? ln) => parseContext.Transformations.AppendLine(ln);

            WriteLine(null);
            WriteLine($"{Resources.A.ParseDirective_token_transformations_lc}:");
            WriteLine(null);

            CaptureTransformation(WriteLine, commandContext.Tokens, $">>> {Resources.A.ParseDirective_from_shell_lc}");

            commandContext.AppConfig.TokenizationEvents.OnTokenTransformation += args =>
            {
                if (args.PreTransformTokens.Count == args.PostTransformTokens.Count &&
                    Enumerable.Range(0, args.PreTransformTokens.Count)
                        .All(i => args.PreTransformTokens[i] == args.PostTransformTokens[i]))
                {
                    CaptureTransformation(WriteLine, null, 
                        $">>> {Resources.A.ParseDirective_after_no_changes(args.Transformation.Name)}");
                }
                else
                {
                    CaptureTransformation(WriteLine, args.PostTransformTokens,
                        $">>> {Resources.A.ParseDirective_after(args.Transformation.Name)}");
                }
            };
        }

        private static void CaptureTransformation(Action<string> writeLine, TokenCollection? args, string description)
        {
            writeLine(description);

            if (args is { })
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
            internal const string IncludeRawCommandLineArgName = "raw";

            internal bool IncludeTokenization;
            internal bool IncludeRawCommandLine;
            internal bool Reported;
            internal readonly StringBuilder Transformations = new();

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
                    IncludeTokenization = settings.ContainsKey(IncludeTransformationsArgName),
                    IncludeRawCommandLine = settings.ContainsKey(IncludeRawCommandLineArgName)
                };
            }
        }
    }
}