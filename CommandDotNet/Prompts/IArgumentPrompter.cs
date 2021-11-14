using System.Collections.Generic;

namespace CommandDotNet.Prompts
{
    public interface IArgumentPrompter
    {
        ICollection<string> PromptForArgumentValues(CommandContext commandContext, IArgument argument, out bool isCancellationRequested);
    }

    // public static class ArgumentPrompterExtensions
    // {
    //     public static T PromptForArgumentValue<T>(this IArgumentPrompter prompter,
    //         CommandContext commandContext,
    //         string alias,
    //         out bool isCancellationRequested)
    //     {
    //         GetValues<T>(prompter, commandContext, alias, out isCancellationRequested);
    //         commandContext.CancellationToken.ThrowIfCancellationRequested();
    //
    //         // TODO: if T is Collection, throw
    //     }
    //
    //     public static ICollection<T> PromptForArgumentValues<T>(this IArgumentPrompter prompter,
    //         CommandContext commandContext,
    //         string alias,
    //         out bool isCancellationRequested)
    //     {
    //         GetValues<T>(prompter, commandContext, alias, out isCancellationRequested);
    //         commandContext.CancellationToken.ThrowIfCancellationRequested();
    //         // TODO: if T is Collection, throw
    //     }
    //
    //     private static ICollection<string> GetValues<T>(IArgumentPrompter prompter, CommandContext commandContext, string alias,
    //         out bool isCancellationRequested)
    //     {
    //         var parseResult = commandContext.ParseResult;
    //         if (parseResult is null)
    //         {
    //             throw new InvalidOperationException("");
    //         }
    //
    //         var argument = parseResult.TargetCommand.FindOrThrow<IArgument>(alias);
    //         return prompter.PromptForArgumentValues(commandContext, argument, out isCancellationRequested);
    //     }
    // }
}