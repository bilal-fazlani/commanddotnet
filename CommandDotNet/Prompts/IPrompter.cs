using System.Collections.Generic;

namespace CommandDotNet.Prompts
{
    public interface IPrompter
    {
        string PromptForValue(string promptText, out bool isCancellationRequested, bool isPassword = false);
        IEnumerable<string> PromptForValues(string promptText, out bool isCancellationRequested, bool isPassword = false);
        bool TryPromptForValue(out string value, out bool isCancellationRequested, string promptText, bool isPassword = false);
        bool TryPromptForValues(out IEnumerable<string> values, out bool isCancellationRequested, string promptText, bool isPassword = false);
    }
}