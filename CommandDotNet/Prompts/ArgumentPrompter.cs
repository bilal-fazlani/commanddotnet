using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;
using JetBrains.Annotations;

namespace CommandDotNet.Prompts;

/// <summary>
/// Contains the logic to prompt for the various types of arguments.
/// </summary>
/// <param name="prompter">The prompter to use for prompting</param>
/// <param name="getPromptTextCallback">Used to customize the generation of the prompt text.</param>
[PublicAPI]
public class ArgumentPrompter(IPrompter prompter,
    Func<CommandContext, IArgument, string>? getPromptTextCallback = null) : IArgumentPrompter
{
    public ICollection<string> PromptForArgumentValues(
        CommandContext commandContext, IArgument argument, out bool isCancellationRequested)
    {
        var argumentName = getPromptTextCallback?.Invoke(commandContext, argument) ?? argument.Name;
        var promptText = $"{argumentName} ({argument.TypeInfo.DisplayName})";
        var isPassword = argument.TypeInfo.UnderlyingType == typeof(Password);
            
        ICollection<string> inputs = new List<string>();
            
        if (argument.Arity.AllowsMany())
        {
            if (prompter.TryPromptForValues(promptText, out var values, out isCancellationRequested, isPassword: isPassword))
            {
                inputs.AddRange(values);
            }
        }
        else
        {
            if (prompter.TryPromptForValue(promptText, out var value, out isCancellationRequested, isPassword: isPassword))
            {
                inputs.Add(value!);
            }
        }

        return inputs;
    }
}