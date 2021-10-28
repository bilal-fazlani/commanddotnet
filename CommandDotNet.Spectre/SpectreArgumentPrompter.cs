using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Prompts;
using Spectre.Console;

namespace CommandDotNet.Spectre
{
    public class SpectreArgumentPrompter : IArgumentPrompter
    {
        private readonly int _defaultPageSize;
        private readonly Func<CommandContext, IArgument, string>? _getPromptTextCallback;

        public SpectreArgumentPrompter(
            int defaultPageSize = 10,
            Func<CommandContext, IArgument, string>? getPromptTextCallback = null)
        {
            _defaultPageSize = defaultPageSize;
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
                if (argument.AllowedValues.Any())
                {
                    var p = new MultiSelectionPrompt<string>()
                        .Title(promptText)
                        .AddChoices(argument.AllowedValues)
                        .PageSize(_defaultPageSize)
                        .MoreChoicesText($"[grey](Move up and down to reveal more {argument.TypeInfo.DisplayName})[/]")
                        .InstructionsText(
                            "[grey](Press [blue]<space>[/] to toggle a fruit, " +
                            "[green]<enter>[/] to accept)[/]");
                    return ansiConsole.Prompt(p);
                }
                else
                {
                    // TODO: how to prompt multiple free hand? use old prompter? Loop Prompt until no value returned? 
                    //       Ask spectre community? 
                    //       comma separated?  first non-alpha-numeric is separater, else comma?
                    // TODO: how to show default? is it the first choice?
                    throw new NotImplementedException("prompting for free-entry lists not yet supported");
                    return Array.Empty<string>();
                }
            }
            else
            {
                if (argument.TypeInfo.Type == typeof(bool))
                {
                    if (defaultValue != null)
                    {
                        ansiConsole.Confirm(promptText, (bool)defaultValue);
                    }
                    else
                    {
                        ansiConsole.Confirm(promptText);
                    }
                }
                if (argument.AllowedValues.Any())
                {
                    var p = new SelectionPrompt<string>()
                        {
                            Title = promptText,
                            PageSize = _defaultPageSize,
                            MoreChoicesText = $"[grey](Move up and down to reveal more {argument.TypeInfo.DisplayName})[/]"
                        }
                        .AddChoices(argument.AllowedValues);
                    
                    // TODO: how to show default? is it the first choice?

                    return new []{ ansiConsole.Prompt(p) };
                }
                else
                {
                    var p = new TextPrompt<string>(promptText)
                    {
                        IsSecret = isPassword,
                        AllowEmpty = argument.Arity.AllowsNone(),
                        ShowDefaultValue = true
                    };
                    if (defaultValue != null)
                    {
                        p.DefaultValue(defaultValue.ToString());
                    }
                    return new[] { ansiConsole.Prompt(p) };
                }
            }
        }
    }
}