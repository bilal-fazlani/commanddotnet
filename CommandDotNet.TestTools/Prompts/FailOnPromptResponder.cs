using System;

namespace CommandDotNet.TestTools.Prompts
{
    public class FailOnPromptResponder : IPromptResponder
    {
        public ConsoleKeyInfo OnReadKey(TestConsole testConsole)
        {
            throw new Exception($"Unexpected prompt for {testConsole.OutLastLine}");
        }
    }
}