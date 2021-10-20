using System.Collections.Generic;

namespace CommandDotNet.Prompts
{
    public interface IPrompter
    {
        string? PromptForValue(string promptText, out bool isCancellationRequested, bool isPassword = false);
        IEnumerable<string> PromptForValues(string promptText, out bool isCancellationRequested, bool isPassword = false);
        bool TryPromptForValue(string promptText, out string? value, out bool isCancellationRequested, bool isPassword = false);
        bool TryPromptForValues(string promptText, out IEnumerable<string> values, out bool isCancellationRequested, bool isPassword = false);
    }
}