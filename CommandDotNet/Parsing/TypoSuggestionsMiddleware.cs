using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Help;
using CommandDotNet.Logging;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    internal static class TypoSuggestionsMiddleware
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

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
                var command = cpe.Command;
                var tokenType = cpe.UnrecognizedArgument.TokenType;

                if ((tokenType == TokenType.Option &&
                     TrySuggest(ctx, cpe, command.Options.Where(o => o.ShowInHelp).ToList(), "option", "--"))
                    || (tokenType == TokenType.Value &&
                        TrySuggest(ctx, cpe, command.Subcommands, "command", null)))
                {
                    return Task.FromResult(1);
                }

                // TODO: suggest other directives? We'd need a list of names which we don't collect atm.
            }

            return next(ctx);
        }

        private static bool TrySuggest(
            CommandContext ctx, CommandParsingException cpe, 
            IReadOnlyCollection<IArgumentNode> argumentNodes, 
            string argumentNodeType, string prefix)
        {
            if (!argumentNodes.Any())
            {
                return false;
            }

            var unrecognizedValue = cpe.UnrecognizedArgument.Value;
            var suggestions = argumentNodes
                .SelectMany(o => o.Aliases.Where(a => a.Length > 1)) // skip short names
                .GetSuggestions(unrecognizedValue);

            if (!suggestions.Any())
            {
                return false;
            }

            var command = cpe.Command;
            var usage = command.GetAppName(ctx.AppConfig.AppSettings.Help) + " " + command.GetPath();

            var @out = ctx.Console.Out;
            @out.WriteLine($"'{unrecognizedValue}' is not a {argumentNodeType}.  See '{usage} --help'");
            @out.WriteLine();
            @out.WriteLine($"Similar {argumentNodeType}s are");
            suggestions.ForEach(name => @out.WriteLine($"   {prefix}{name}"));

            return true;
        }

        private static List<string> GetSuggestions(this IEnumerable<string> names, string unrecognizedValue)
        {
            names = names.Where(n => !n.IsNullOrWhitespace()).ToList();
            var tuples = names.Select(name =>
            {
                var distance = Levenshtein.ComputeDistance(unrecognizedValue, name);
                var startsWith = GetStartsWithLength(unrecognizedValue, name);
                var sameness = GetSamenessLength(unrecognizedValue, name);
                return (name, distance, startsWith, sameness, score: distance + -startsWith + -sameness);
            }).ToList();

            if (Log.IsDebugEnabled())
            {
                var withScores = tuples
                    .OrderBy(v => v.score)
                    .Select(v => $"  {v.name}  - distance:{v.distance} startsWith:{v.startsWith} sameness:{v.sameness} score:{v.score}");
                Log.Debug($"scores:{Environment.NewLine}{withScores.ToCsv(Environment.NewLine)}");
            }

            bool IsSimilarEnough((string name, int distance, int startsWith, int sameness, int score) valueTuple)
            {
                return !(valueTuple.sameness == 0 && valueTuple.startsWith == 0 & valueTuple.distance > 3);
            }

            return tuples
                .Where(v => v.score < 5)
                .Where(IsSimilarEnough)
                .OrderBy(v => v.distance + -v.startsWith + -v.sameness)
                .Select(s => s.name)
                .Take(5)
                .ToList();
        }

        private static int GetStartsWithLength(string first, string second)
        {
            for (int i = 0; i < Math.Min(first.Length, second.Length); i++)
            {
                if (first[i] != second[i])
                    return i;
            }

            return first.Length;
        }

        private static int GetSamenessLength(string first, string second)
        {
            int same = 0;
            for (int i = 0; i < Math.Min(first.Length, second.Length); i++)
            {
                if (first[i] == second[i])
                    same++;
            }

            return same;
        }
    }
}