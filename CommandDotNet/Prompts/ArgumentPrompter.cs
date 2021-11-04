using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.Prompts
{
    /// <summary>
    /// Contains the logic to prompt for the various types of arguments.
    /// </summary>
    public class ArgumentPrompter : IArgumentPrompter
    {
        private readonly Func<CommandContext, IArgument, string>? _getPromptTextCallback;
        private readonly IPrompter _prompter;

        /// <summary>
        /// Contains the logic to prompt for the various types of arguments.
        /// </summary>
        /// <param name="prompter">The prompter to use for prompting</param>
        /// <param name="getPromptTextCallback">Used to customize the generation of the prompt text.</param>
        public ArgumentPrompter(
            IPrompter prompter,
            Func<CommandContext, IArgument, string>? getPromptTextCallback = null)
        {
            _prompter = prompter;
            _getPromptTextCallback = getPromptTextCallback;
        }

        public ICollection<string> PromptForArgumentValues(
            CommandContext commandContext, IArgument argument, out bool isCancellationRequested)
        {
            var argumentName = _getPromptTextCallback?.Invoke(commandContext, argument) ?? argument.Name;
            var promptText = $"{argumentName} ({argument.TypeInfo.DisplayName})";
            var isPassword = argument.TypeInfo.UnderlyingType == typeof(Password);
            
            ICollection<string> inputs = new List<string>();
            
            if (argument.Arity.AllowsMany())
            {
                if (_prompter.TryPromptForValues(promptText, out var values, out isCancellationRequested, isPassword: isPassword))
                {
                    inputs.AddRange(values);
                }
            }
            else
            {
                if (_prompter.TryPromptForValue(promptText, out var value, out isCancellationRequested, isPassword: isPassword))
                {
                    inputs.Add(value!);
                }
            }

            return inputs;
        }
    }
}