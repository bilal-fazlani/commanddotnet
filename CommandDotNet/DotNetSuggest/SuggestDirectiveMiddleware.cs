using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.DotNetSuggest;

internal static class SuggestDirectiveMiddleware
{
    internal static AppRunner UseSuggestDirective_Experimental(this AppRunner appRunner)
    {
        return appRunner.Configure(c => 
            c.UseMiddleware(SuggestDirective, MiddlewareSteps.AutoSuggest.Directive));
    }

    private static Task<int> SuggestDirective(CommandContext ctx, ExecutionDelegate next)
    {
        if (!ctx.Tokens.TryGetDirective("suggest", out string? value))
        {
            return next(ctx);
        }

        // do not show help for suggest directive
        ctx.ShowHelpOnExit = false;

        // var parts = value.Split(':');
        // int position = parts.Length == 1 ? 0 : int.Parse(parts[1]);
        
        // single dash could be valid for negative numbers
        // but probably not for someone looking for auto-suggest
        var lastToken = ctx.Original.Args.LastOrDefault();
        var lookingForOptions = lastToken is not null 
                                && (lastToken.StartsWith("-") || lastToken.StartsWith("/"));
        var useBackslash = lastToken?.StartsWith("/") ?? false;

        if (ctx.ParseResult!.ParseError == null)
        {
            var command = ctx.ParseResult.TargetCommand;
            var suggestions = ToOptionSuggestions(command, useBackslash);

            if (!lookingForOptions)
            {
                if (ctx.ParseResult.NextAvailableOperand is not null)
                {
                    suggestions = Concat(suggestions, ctx.ParseResult.NextAvailableOperand.AllowedValues);
                }

                suggestions = Concat(suggestions, ToSubcommandSuggestions(command, ctx.ParseResult));
            }
            
            Report(ctx.Console, suggestions);

            return ExitCodes.Success;
        }
        
        if (ctx.ParseResult.TokensEvaluatedCount < ctx.Tokens.Arguments.Count)
        {
            // The errors could be the result of typos for arguments earlier in the provided args list.
            // Returning hints for those arguments would not give the user the results they are expecting.
            // In those cases, it's best to return no suggestions like other apps do.
            return ExitCodes.Error;
        }
        
        switch (ctx.ParseResult.ParseError)
        {
            case UnrecognizedOptionParseError unrecognizedOption:
                Report(ctx.Console, 
                    ToOptionSuggestions(unrecognizedOption.Command, useBackslash), 
                    unrecognizedOption.Token.Value);
                break;
            case UnrecognizedArgumentParseError unrecognizedArgument:
                // TODO: include allowed values for first operand (what is the scenario?)
                Report(ctx.Console, 
                     ToSubcommandSuggestions(unrecognizedArgument.Command, ctx.ParseResult), 
                    unrecognizedArgument.Token.Value);
                break;
            case NotAllowedValueParseError notAllowedValue:
                var suggestions = lookingForOptions
                    ? ToOptionSuggestions(notAllowedValue.Command, useBackslash)
                    : Concat(notAllowedValue.Argument.AllowedValues,
                        ToSubcommandSuggestions(notAllowedValue.Command, ctx.ParseResult));
                Report(ctx.Console, suggestions, notAllowedValue.Token.Value);
                break;
            case MissingOptionValueParseError missingOptionValueParseError:
                Report(ctx.Console, 
                    missingOptionValueParseError.Option.AllowedValues);
                break;
            default:
                ctx.Console.WriteLine($"unhandled parser error {ctx.ParseResult.ParseError.GetType().Name} {ctx.ParseResult.ParseError.Message}");
                break;
        }

        return ExitCodes.Success;
    }

    private static IEnumerable<string> ToSubcommandSuggestions(Command command,
        ParseResult parseResult) => 
        parseResult.IsCommandIdentified 
            ? Enumerable.Empty<string>() 
            : command.Subcommands.SelectMany(c => c.Aliases);

    private static IEnumerable<string> ToOptionSuggestions(Command command, bool useBackslash) => 
        command.Options
            .Where(o => !o.Hidden)
            .Where(o => o.Arity.AllowsMany() || !o.HasValueFromInput())
            .SelectMany(o => o.Aliases.Where(a => a.Length > 1))
            .Select(a => useBackslash ? $"/{a}" : $"--{a}");

    private static IEnumerable<string> Concat(params IEnumerable<string>[] suggestions) => 
        suggestions.Aggregate(
                Enumerable.Empty<string>(), 
                (current, s) => current.Concat(s));

    private static void Report(IConsole console, IEnumerable<string> suggestions, string? root = null)
    {
        if (root is not null)
        {
            suggestions = suggestions.Where(s => s.StartsWith(root));
        }
        suggestions.OrderBy(s => s).ForEach(console.WriteLine);
    }
}