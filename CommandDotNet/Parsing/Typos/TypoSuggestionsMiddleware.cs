using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Builders;
using CommandDotNet.Execution;
using CommandDotNet.Help;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing.Typos
{
    internal static class TypoSuggestionsMiddleware
    {
        internal static AppRunner UseTypoSuggestions(AppRunner appRunner, int maxSuggestionCount)
        {
            if (maxSuggestionCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSuggestionCount), $"{maxSuggestionCount} must be > 0");
            }

            return appRunner.Configure(c =>
            {
                c.Services.Add(new Config {MaxSuggestionCount = maxSuggestionCount});   
                c.UseMiddleware(TypoSuggest, MiddlewareSteps.TypoSuggest);
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

                IEnumerable<string> GetOptionNames()
                {
                    // skip short names
                    return command.Options
                        .Where(o => !o.Hidden)
                        .SelectMany(o => o.Aliases.Where(a => a.Length > 1));
                }
                IEnumerable<string> GetSubcommandNames()
                {
                    return command.Subcommands.SelectMany(o => o.Aliases);
                }

                if ((tokenType == TokenType.Option &&
                     TrySuggest(ctx, cpe, GetOptionNames().ToList(), "option", "--"))
                    || (tokenType == TokenType.Value &&
                        TrySuggest(ctx, cpe, GetSubcommandNames().ToList(), "command", null)))
                {
                    return ExitCodes.Error;
                }

                // TODO: suggest other directives? We'd need a list of names which we don't collect atm.
            }

            return next(ctx);
        }

        private static bool TrySuggest(
            CommandContext ctx, CommandParsingException cpe, 
            ICollection<string> candidates, 
            string argumentNodeType, string prefix)
        {
            if (!candidates.Any())
            {
                return false;
            }

            var config = ctx.AppConfig.Services.Get<Config>();

            var typo = cpe.UnrecognizedArgument.Value;

            // TODO: allowed values
            //   if the next value should be a value for an option, include allowed values for the option.
            //   else include allowed values of the next expected operand
            //      fyi: the next expected operand could be a list operand that already has values.
            //           logic should remain the same, just be sure to include this test case

            var suggestions = candidates
                .RankAndTrimSuggestions(typo, config.MaxSuggestionCount)
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
    }
}