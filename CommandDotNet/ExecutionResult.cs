using System.Collections.Generic;
using CommandDotNet.Parsing;

namespace CommandDotNet
{
    public class ExecutionResult
    {
        public bool ShouldExit { get; private set; }
        public int ExitCode { get; private set; }

        public string[] OriginalArgs { get; }
        public TokenCollection OriginalTokens { get; set; }

        public TokenCollection FinalTokens { get; set; }

        public ICommand RootCommand { get; set; }

        public ParseResult ParseResult { get; set; }

        public IContextData ContextData { get; } = new ContextData();

        public ExecutionResult(string[] originalArgs)
        {
            OriginalArgs = originalArgs;
        }

        public void ShouldExitWithCode(int exitCode)
        {
            ShouldExit = true;
            ExitCode = exitCode;
        }
    }
}