using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    internal static class TypoSuggestionsMiddleware
    {
        internal static AppRunner UseTypoSuggestions(AppRunner appRunner)
        {
            // -1 to ensure this middleware runs before any prompting so the value won't appear null
            return appRunner.Configure(c => c.UseMiddleware(Middleware,
                MiddlewareSteps.TypoSuggest.Stage, MiddlewareSteps.TypoSuggest.Order));
        }

        private static Task<int> Middleware(CommandContext ctx, ExecutionDelegate next)
        {
            if (ctx.ParseResult.ParseError != null 
                && ctx.ParseResult.ParseError is CommandParsingException cpe 
                && cpe.UnrecognizedArgument != null)
            {
                var @out = ctx.Console.Out;
                var command = cpe.Command;
                var usage = command.GetAppName(ctx.AppConfig.AppSettings.Help) + " " +  command.GetPath();

                var unrecognizedValue = cpe.UnrecognizedArgument.Value;

                switch (cpe.UnrecognizedArgument.TokenType)
                {
                    case TokenType.Option:
                        @out.WriteLine($"{cpe.UnrecognizedArgument.RawValue} is not an option.  See '{usage} --help'");
                        if (command.Options.Any())
                        {
                            if (cpe.UnrecognizedArgument.OptionTokenType.IsLong)
                            {
                                command.Options
                                    .Where(o => o.ShowInHelp)
                                    .Select(o => o.LongName)
                                    .WriteClosestPossibleMatches("option", unrecognizedValue, "--", ctx.Console);
                            }
                            else
                            {
                                command.Options
                                    .Where(o => o.ShowInHelp)
                                    .Select(o => o.ShortName.ToString())
                                    .WriteClosestPossibleMatches("option", unrecognizedValue, "-", ctx.Console);
                            }
                        }
                        break;
                    case TokenType.Value:
                        @out.WriteLine($"{cpe.UnrecognizedArgument.RawValue} is not a command.  See '{usage} --help'");
                        if (command.Subcommands.Any())
                        {
                            command.Subcommands
                                .Select(o => o.Name)
                                .WriteClosestPossibleMatches("command", unrecognizedValue, null, ctx.Console);
                        }
                        break;
                    case TokenType.Directive:
                        // TODO: suggest other directives? We'd need a list of names which we don't collect atm.
                    default:
                        throw new ArgumentOutOfRangeException($"unknown {nameof(TokenType)}: {cpe.UnrecognizedArgument.TokenType}");
                }
                return Task.FromResult(1);
            }

            return next(ctx);
        }

        private static void WriteClosestPossibleMatches(this IEnumerable<string> names, 
            string title, string unrecognizedValue, string prefix, IConsole console)
        {

            names = names.Where(n => !n.IsNullOrWhitespace()).ToList();
            console.Out.WriteLine();
            console.Out.WriteLine($"The most similar {title} is");
            names.Select(name => (
                    name, 
                    distance: Levenshtein.ComputeDistance(unrecognizedValue, name),
                    startsWith: GetStartsWithDistance(unrecognizedValue, name),
                    sameness: GetSamenessDistance(unrecognizedValue, name),
                    score: 0))
                .OrderBy(v => v.distance / (v.startsWith + v.sameness + 1))
                .ThenBy(v => v.distance + -v.sameness)
                .ForEach(v => console.Out.WriteLine($"   {prefix}{v.name} (score1: {v.distance / (v.startsWith + v.sameness + 1)} score2: {v.distance + -v.sameness})"));
        }

        private static int GetStartsWithDistance(string first, string second)
        {
            for (int i = 0; i < Math.Min(first.Length, second.Length); i++)
            {
                if (first[i] != second[i])
                    return i;
            }

            return first.Length;
        }

        private static int GetSamenessDistance(string first, string second)
        {
            int same = 0;
            for (int i = 0; i < Math.Min(first.Length, second.Length); i++)
            {
                if (first[i] != second[i])
                    same++;
            }

            return same;
        }
    }
}