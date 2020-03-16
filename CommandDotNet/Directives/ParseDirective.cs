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
                c.UseMiddleware(ConfigureParseReportByTokenTransform, MiddlewareStages.PreTokenize);
                c.UseMiddleware(ExitAfterTokenizationWhenParseDirective, MiddlewareStages.Tokenize, MiddlewareSteps.CreateRootCommand.Order - 100);
                c.UseMiddleware(ParseReportByArg, MiddlewareStages.BindValues, MiddlewareSteps.BindValues.Order + 100);
            });
        }

        private class Settings
        {
            internal bool Help;
            internal bool ByArg;
            internal bool TokensOnly;
            internal bool Verbose;

            internal static Settings Parse(string value)
            {
                var parts = value.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length <= 1)
                {
                    return new Settings {ByArg = true};
                }

                var settings = parts[1].Split(';')
                    .Select(p => p.Split('='))
                    .ToDictionary(p => p[0], p => p.Length > 1 ? p[1] : null, StringComparer.OrdinalIgnoreCase);

                return new Settings
                {
                    Help = settings.ContainsKey("help"),
                    ByArg = !settings.ContainsKey("tokens"),
                    TokensOnly = settings.ContainsKey("tokens"),
                    Verbose = settings.ContainsKey("verbose")
                };

            }
        }

        // adapted from https://github.com/dotnet/command-line-api directives
            private static Task<int> ConfigureParseReportByTokenTransform(CommandContext commandContext, ExecutionDelegate next)
        {
            if (commandContext.Tokens.TryGetDirective("parse", out string value))
            {
                var appConfig = commandContext.AppConfig;

                var settings = Settings.Parse(value);
                commandContext.Services.AddOrUpdate(settings);

                var console = commandContext.Console;
                if (!settings.Help)
                {
                    console.Out.WriteLine("use [parse:help] to see additional parse options");
                }
                else
                {
                    console.Out.WriteLine("usage [parse:{options}]");
                    console.Out.WriteLine(" [parse:tokens] to see tokenization results. Does not include argument mapping.");
                    console.Out.WriteLine(" [parse:tokens;verbose] to see tokenization results after each token transformation.");
                    return Task.FromResult(0);
                }

                if (settings.TokensOnly)
                {
                    ReportByStage(appConfig, commandContext, console, settings);
                }
            }

            return next(commandContext);
        }

        private static void ReportByStage(
            AppConfig appConfig, CommandContext commandContext, IConsole console, Settings settings)
        {
            ReportTransformation(console, commandContext.Tokens, ">>> from shell");

            if (settings.Verbose)
            {
                appConfig.TokenizationEvents.OnTokenTransformation += args =>
                {
                    if (args.PreTransformTokens.Count == args.PostTransformTokens.Count &&
                        Enumerable.Range(0, args.PreTransformTokens.Count)
                            .All(i => args.PreTransformTokens[i] == args.PostTransformTokens[i]))
                    {
                        ReportTransformation(console, null, $">>> no changes after: {args.Transformation.Name}");
                    }
                    else
                    {
                        ReportTransformation(console, args.PostTransformTokens,
                            $">>> transformed after: {args.Transformation.Name}");
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

        private static Task<int> ParseReportByArg(CommandContext commandContext, ExecutionDelegate next)
        {
            var settings = commandContext.Services.Get<Settings>();
            if (settings?.ByArg ?? false)
            {
                var console = commandContext.Console;
                var command = commandContext.ParseResult.TargetCommand;

                var arguments = command
                    .AllArguments(includeInterceptorOptions: true, excludeHiddenOptions: true)
                    .OrderBy(a => a is Option)
                    .ThenBy(a => a.Name)
                    .ToList();
                
                string PrintRecursively(ValueFromToken vft)
                {
                    return $"{vft.Value} (from: {Recurse(vft)})";
                }

                string Recurse(ValueFromToken vft)
                {
                    if ((vft.OptionToken?.SourceToken ?? vft.ValueToken?.SourceToken) == null)
                    {
                        return Print(vft);
                    }

                    return vft.TokensSourceToken == null 
                        ? Print(vft)
                        : $"{Recurse(new ValueFromToken(null, vft.ValueToken?.SourceToken, vft.OptionToken?.SourceToken))} -> {Print(vft)}";
                }

                string Print(ValueFromToken vft)
                {
                    return $"{vft.OptionToken?.RawValue} {vft.ValueToken?.RawValue}".Trim();
                }

                Action<string> writeln = console.Out.WriteLine;
                writeln(null);
                writeln(command.GetPath());
                writeln(null);
                foreach (var argument in arguments)
                {

                    writeln($"{argument.Name} <{argument.TypeInfo.DisplayName ?? (argument.Arity.AllowsNone() ? "Flag" : null)}>");
                    writeln($"  value={argument.Value?.ValueToString()}");
                    if (argument.InputValues?.Any() ?? false)
                    {
                        var values = argument.InputValues
                            .Select(iv => iv.Source == Constants.InputValueSources.Argument
                                ? $" {iv.ValuesFromTokens.Select(PrintRecursively).ToCsv()}"
                                : $" [{iv.Source}] {iv.ValuesFromTokens.Select(PrintRecursively).ToCsv()}")
                            .ToList();
                        if (values.Count == 1)
                        {
                            writeln($"  inputs:{values.Single()}");
                        }
                        else
                        {
                            writeln("  inputs:");
                            values.ForEach(v => writeln($"    {v}"));
                        }
                    }
                    if (argument.Default != null)
                    {
                        if (argument.Default.Source.StartsWith("app."))
                        {
                            writeln($"  default: {argument.Default.Value.ValueToString()}");
                        }
                        else
                        {
                            writeln($"  default: source={argument.Default.Source} key={argument.Default.Key}: {argument.Default.Value.ValueToString()}");
                        }
                    }
                }
            }

            return Task.FromResult(0);
        }

        private static Task<int> ExitAfterTokenizationWhenParseDirective(CommandContext commandContext, ExecutionDelegate next)
        {
            var settings = commandContext.Services.Get<Settings>();
            return settings != null && !settings.ByArg
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