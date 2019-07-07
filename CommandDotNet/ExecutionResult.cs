using System.Collections.Generic;
using CommandDotNet.Parsing;

namespace CommandDotNet
{
    public class ExecutionResult
    {
        public bool ShouldExit { get; private set; }
        public int ExitCode { get; private set; }

        public string[] OriginalArgs { get; }
        public TokenCollection OriginalTokens { get; }

        public TokenCollection Tokens { get; set; }

        public ParseResult ParseResult { get; set; }

        public IContextData ContextData { get; } = new ContextData();

        public ExecutionResult(string[] originalArgs, TokenCollection originalTokens)
        {
            OriginalArgs = originalArgs;
            OriginalTokens = originalTokens;
            Tokens = originalTokens;
        }

        public void ShouldExitWithCode(int exitCode)
        {
            ShouldExit = true;
            ExitCode = exitCode;
        }
    }
}