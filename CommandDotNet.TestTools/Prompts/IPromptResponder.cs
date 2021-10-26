using System;

namespace CommandDotNet.TestTools.Prompts
{
    public interface IPromptResponder
    {
        ConsoleKeyInfo OnReadKey(ITestConsole testConsole);
    }
}