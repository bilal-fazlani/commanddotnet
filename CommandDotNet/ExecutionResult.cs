using CommandDotNet.Parsing;

namespace CommandDotNet
{
    public class ExecutionResult
    {
        public bool ShouldExit { get; private set; }
        public int ExitCode { get; private set; }

        public OriginalInput Original { get; }

        public TokenCollection Tokens { get; set; }

        public ParserConfig ParserConfig { get; set; }

        public ParseResult ParseResult { get; set; }

        public AppSettings AppSettings { get; }

        public IContextData ContextData { get; } = new ContextData();

        public ExecutionResult(string[] originalArgs, TokenCollection originalTokens, AppSettings appSettings)
        {
            Original = new OriginalInput(originalArgs, originalTokens);
            Tokens = originalTokens;
            AppSettings = appSettings;
        }

        public void ShouldExitWithCode(int exitCode)
        {
            ShouldExit = true;
            ExitCode = exitCode;
        }
    }
}