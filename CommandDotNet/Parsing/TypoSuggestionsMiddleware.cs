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

        internal static AppRunner UseTypoSuggestions(AppRunner appRunner, int maxSuggestionCount)
        {
            if (maxSuggestionCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSuggestionCount), $"{maxSuggestionCount} must be > 0");
            }

            return appRunner.Configure(c =>
            {
                c.Services.Add(new Config {MaxSuggestionCount = maxSuggestionCount});   
                c.UseMiddleware(TypoSuggest,
                    MiddlewareSteps.TypoSuggest.Stage, MiddlewareSteps.TypoSuggest.Order);
            });
        }

        private class Config
        {
            public int MaxSuggestionCount;
        }

        private static Task<int> TypoSuggest(CommandContext ctx, ExecutionDelegate next)
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

            var config = ctx.AppConfig.Services.Get<Config>();

            var typo = cpe.UnrecognizedArgument.Value;
            var suggestions = argumentNodes
                .SelectMany(o => o.Aliases.Where(a => a.Length > 1)) // skip short names
                .GetSuggestions(typo, config.MaxSuggestionCount)
                .ToList();

            if (!suggestions.Any())
            {
                return false;
            }

            var command = cpe.Command;
            var usage = command.GetAppName(ctx.AppConfig.AppSettings.Help) + " " + command.GetPath();

            var @out = ctx.Console.Out;
            @out.WriteLine($"'{typo}' is not a {argumentNodeType}.  See '{usage} --help'");
            @out.WriteLine();
            @out.WriteLine($"Similar {argumentNodeType}s are");
            suggestions.ForEach(name => @out.WriteLine($"   {prefix}{name}"));

            return true;
        }

        internal static IEnumerable<string> GetSuggestions(this IEnumerable<string> names, 
            string typo, int maxSuggestionCount)
        {
            var tuples = names
                .Where(n => !n.IsNullOrWhitespace())
                .Select(name =>
                {
                    var rearranged = GetRearranged(typo).ToList();

                    var distance = -Levenshtein.ComputeDistance(typo, name);
                    var startsWith = GetStartsWithLength(typo, name);
                    var sameness = rearranged.Max(n => GetSamenessLength(n, name));
                    sameness = sameness == typo.Length 
                        ? sameness * 2 //found possible match of compound words out-of-order
                        : Math.Max(sameness, GetContainsLength(typo, name));
                    return (name, distance, startsWith, sameness, score: distance + startsWith + sameness);
                })
                .OrderByDescending(v => v.score)
                .ThenBy(v => v.startsWith)
                .ThenBy(v => v.sameness)
                .ThenBy(v => v.name)
                .ToList();

            if (Log.IsDebugEnabled())
            {
                var withScores = tuples
                    .Select(v => $"  {v.name}  - distance:{v.distance} startsWith:{v.startsWith} sameness:{v.sameness} score:{v.score}");
                Log.Debug($"scores:{Environment.NewLine}{withScores.ToCsv(Environment.NewLine)}");
            }

            bool IsSimilarEnough((string name, int distance, int startsWith, int sameness, int score) valueTuple)
            {
                return !(valueTuple.sameness == 0 && valueTuple.startsWith == 0 & valueTuple.distance < -3);
            }

            return tuples
                .Where(v => v.score > -3)
                .Where(IsSimilarEnough)
                .Select(s => s.name)
                .Take(maxSuggestionCount);
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

        private static int GetContainsLength(string first, string second)
        {
            var (small, large) = first.Length > second.Length ? (second, first) : (first, second);
            return large.ToLower().Contains(small.ToLower()) ? small.Length : 0;
        }
        
        private static IEnumerable<string> GetRearranged(string typo)
        {
            for (int i = 0; i < typo.Length; i++)
            {
                string name = "";

                if (i < typo.Length - 1)
                {
                    name += typo.Substring(i);
                }
                if (i > 0)
                {
                    name += typo.Substring(0, i);
                }

                yield return name;
            }
        }
    }
}