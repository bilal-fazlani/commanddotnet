using System;
using System.Collections.Generic;
using CommandDotNet.Prompts;
using JetBrains.Annotations;
using Spectre.Console;

namespace CommandDotNet.Spectre;

/// <summary>
/// Contains the logic to prompt for the various types of arguments.
/// </summary>
[PublicAPI]
public class SpectreArgumentPrompter : IArgumentPrompter
{
    private readonly int _pageSize;
    private readonly Func<CommandContext, IArgument, string>? _getPromptTextCallback;

    /// <summary>
    /// Contains the logic to prompt for the various types of arguments.
    /// </summary>
    /// <param name="pageSize">the page size for selection lists.</param>
    /// <param name="getPromptTextCallback">Used to customize the generation of the prompt text.</param>
    public SpectreArgumentPrompter(
        int pageSize = 10,
        Func<CommandContext, IArgument, string>? getPromptTextCallback = null)
    {
        _pageSize = pageSize;
        _getPromptTextCallback = getPromptTextCallback;
    }

    public virtual ICollection<string> PromptForArgumentValues(
        CommandContext ctx, IArgument argument, out bool isCancellationRequested)
    {
        isCancellationRequested = false;

        var argumentName = _getPromptTextCallback?.Invoke(ctx, argument) ?? argument.Name;
        var promptText = $"{argumentName} ({argument.TypeInfo.DisplayName})";
        var isPassword = argument.TypeInfo.UnderlyingType == typeof(Password);
        var defaultValue = argument.Default?.Value;
            
        var ansiConsole = ctx.Services.GetOrThrow<IAnsiConsole>();
        // https://spectreconsole.net/prompts/

        if (argument.Arity.AllowsMany())
        {
            if (argument.AllowedValues.Count == 0) return MultiPrompt(ansiConsole, promptText);
            
            // TODO: how to show default? is it the first choice?
            var p = new MultiSelectionPrompt<string>
                {
                    MoreChoicesText = Resources.A.Selection_paging_instructions(argument.Name),
                    InstructionsText = Resources.A.MultiSelection_selection_instructions(argument.Name)
                }
                .Title(promptText)
                .AddChoices(argument.AllowedValues)
                .PageSize(_pageSize);

            return ansiConsole.Prompt(p);

        }

        if (argument.TypeInfo.Type == typeof(bool))
        {
            var result = defaultValue != null
                ? ansiConsole.Confirm(promptText, (bool)defaultValue)
                : ansiConsole.Confirm(promptText);

            return [result.ToString()];
        }
        
        if (argument.AllowedValues.Count != 0)
        {
            var p = new SelectionPrompt<string>()
                {
                    Title = promptText,
                    PageSize = _pageSize,
                    MoreChoicesText = Resources.A.Selection_paging_instructions(argument.Name)
                }
                .AddChoices(argument.AllowedValues);

            // TODO: how to show default? is it the first choice?

            return [ansiConsole.Prompt(p)];
        }
        else
        {
            var p = new TextPrompt<string>(promptText)
            {
                IsSecret = isPassword,
                AllowEmpty = argument.Arity.RequiresNone(),
                ShowDefaultValue = true
            };
            if (defaultValue != null)
            {
                p.DefaultValue(defaultValue.ToString()!);
            }
            return [ansiConsole.Prompt(p)];
        }
    }

    private static List<string> MultiPrompt(IAnsiConsole ansiConsole, string prompt)
    {
        var answers = new List<string>();
        ansiConsole.WriteLine(prompt);
        while (true)
        {
            var textPrompt = new TextPrompt<string>("> ") { AllowEmpty = true };
            var answer = ansiConsole.Prompt(textPrompt);
            if (string.IsNullOrEmpty(answer))
            {
                return answers;
            }

            answers.Add(answer);
        }
    }
}