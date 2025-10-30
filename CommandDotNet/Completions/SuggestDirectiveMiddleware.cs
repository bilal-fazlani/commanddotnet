using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Directives;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet.Completions;

/// <summary>
/// Middleware that handles [suggest] directive to provide shell completion suggestions.
/// Uses ParseResult to provide intelligent, context-aware suggestions based on parse state.
/// Handles both successful parses and parse errors to provide contextual completions.
/// </summary>
internal static class SuggestDirectiveMiddleware
{
    internal static AppRunner UseSuggestDirective(this AppRunner appRunner)
    {
        return appRunner.Configure(c =>
        {
            c.UseMiddleware(SuggestDirective, MiddlewareSteps.Suggest);
        });
    }

    private static Task<int> SuggestDirective(CommandContext ctx, ExecutionDelegate next)
    {
        if (!ctx.Tokens.TryGetDirective("suggest", out var _))
        {
            return next(ctx);
        }

        // Prevent help from showing
        ctx.ShowHelpOnExit = false;

        // When parse is successful, the last token was consumed and we're suggesting what comes next
        // When there's a parse error, the last token is the partial/invalid value we're completing
        string? partialValue;
        var lookingForOptions = false;
        var useBackslash = false;

        if (ctx.ParseResult?.ParseError == null && ctx.ParseResult != null)
        {
            // Successful parse - suggest based on current context
            var command = ctx.ParseResult.TargetCommand;
            var suggestions = ToOptionSuggestions(command, useBackslash);

            if (!lookingForOptions)
            {
                // Include operand allowed values for first unfilled operand
                var nextOperand = command.Operands.FirstOrDefault(o => !o.HasValueFromInput());
                if (nextOperand is not null)
                {
                    suggestions = Concat(suggestions, GetAllowedValues(nextOperand));
                }

                // Include subcommands if available
                if (command.Subcommands.Any())
                {
                    suggestions = Concat(suggestions, ToSubcommandSuggestions(command));
                }
            }
            
            // No partial value to filter by - show all suggestions
            Report(ctx.Console, suggestions);
            return ExitCodes.SuccessAsync;
        }

        if (ctx.ParseResult == null)
        {
            return ExitCodes.ErrorAsync;
        }

        // Handle parse errors - the error token is the partial value being completed
        switch (ctx.ParseResult.ParseError)
        {
            case UnrecognizedOptionParseError unrecognizedOption:
                partialValue = unrecognizedOption.Token.Value;
                lookingForOptions = partialValue.StartsWith("-") || partialValue.StartsWith("/");
                useBackslash = partialValue.StartsWith("/");
                Report(ctx.Console, 
                    ToOptionSuggestions(unrecognizedOption.Command, useBackslash), 
                    partialValue);
                break;
            case UnrecognizedArgumentParseError unrecognizedArgument:
                partialValue = unrecognizedArgument.Token.Value;
                Report(ctx.Console, 
                     ToSubcommandSuggestions(unrecognizedArgument.Command), 
                    partialValue);
                break;
            case NotAllowedValueParseError notAllowedValue:
                partialValue = notAllowedValue.Token.Value;
                lookingForOptions = partialValue.StartsWith("-") || partialValue.StartsWith("/");
                useBackslash = partialValue.StartsWith("/");
                var suggestions = lookingForOptions
                    ? ToOptionSuggestions(notAllowedValue.Command, useBackslash)
                    : Concat(GetAllowedValues(notAllowedValue.Argument),
                        ToSubcommandSuggestions(notAllowedValue.Command));
                Report(ctx.Console, suggestions, partialValue);
                break;
            case MissingOptionValueParseError missingOptionValueParseError:
                Report(ctx.Console, 
                    GetAllowedValues(missingOptionValueParseError.Option));
                break;
            default:
                // Unknown parse error - no suggestions
                break;
        }

        return ExitCodes.SuccessAsync;
    }

    private static IEnumerable<string> ToSubcommandSuggestions(Command command) => 
        command.Subcommands
            .Where(c => !c.Name.StartsWith("__")) // Skip hidden commands
            .SelectMany(c => c.Aliases);

    private static IEnumerable<string> ToOptionSuggestions(Command command, bool useBackslash) => 
        command.Options
            .Where(o => !o.Hidden || o.IsMiddlewareOption) // Show middleware options like --help even if hidden
            .Where(o => o.Arity.AllowsMany() || !o.HasValueFromInput())
            .SelectMany(o => o.Aliases.Where(a => a.Length > 1))
            .Select(a => useBackslash ? $"/{a}" : $"--{a}");

    private static IEnumerable<string> GetAllowedValues(IArgument argument)
    {
        // Check for explicit AllowedValues
        if (argument.AllowedValues != null && argument.AllowedValues.Any())
        {
            return argument.AllowedValues;
        }

        // Check for enum types
        var underlyingType = argument.TypeInfo.UnderlyingType;
        if (underlyingType.IsEnum)
        {
            return System.Enum.GetNames(underlyingType);
        }

        return Enumerable.Empty<string>();
    }

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
