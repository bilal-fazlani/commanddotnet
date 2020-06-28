using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Parsing.Typos
{
    internal static class SuggestionRankingExtensions
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        internal static IEnumerable<string> RankAndTrimSuggestions(
            this IEnumerable<string> suggestions, string typo, int maxSuggestionCount)
        {
            var tuples = suggestions
                .Where(n => !n.IsNullOrWhitespace())
                .Select(name =>
                {
                    var distance = -Levenshtein.ComputeDistance(typo, name);
                    var startsWith = GetStartsWithLength(typo, name);
                    var sameness = GetMaxSamenessLength(typo, name);
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

        private static int GetMaxSamenessLength(string typo, string name)
        {
            var rearrangedSameness = GetRearrangedSameness(typo, name);

            return rearrangedSameness == typo.Length
                ? rearrangedSameness * 2 //found possible match of compound words out-of-order
                : Math.Max(rearrangedSameness, GetContainsLength(typo, name));
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

        private static int GetRearrangedSameness(string typo, string name)
        {
            // checks cases where typo could be out-of-order spelling
            // eg: nameuser -> username
            int maxSameness = 0;

            for (int i = 0; i < typo.Length; i++)
            {
                string rearrangedName = "";

                if (i < typo.Length - 1)
                {
                    rearrangedName += typo.Substring(i);
                }
                if (i > 0)
                {
                    rearrangedName += typo.Substring(0, i);
                }

                maxSameness = Math.Max(maxSameness, GetSamenessLength(rearrangedName, name));
            }

            return maxSameness;
        }
    }
}