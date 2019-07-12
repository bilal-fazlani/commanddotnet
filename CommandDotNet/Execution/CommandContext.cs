using CommandDotNet.Parsing;

namespace CommandDotNet.Execution
{
    public class CommandContext
    {
        public bool ShouldExit { get; private set; }
        public int ExitCode { get; private set; }

        public OriginalInput Original { get; }

        public TokenCollection Tokens { get; set; }

        public ParseResult ParseResult { get; set; }
        
        public ExecutionConfig ExecutionConfig { get; }
        
        public AppSettings AppSettings { get; }

        public IContextData ContextData { get; } = new ContextData();

        public CommandContext(
            string[] originalArgs, 
            TokenCollection originalTokens, 
            AppSettings appSettings,
            ExecutionConfig executionConfig)
        {
            Original = new OriginalInput(originalArgs, originalTokens);
            Tokens = originalTokens;
            AppSettings = appSettings;
            ExecutionConfig = executionConfig;
        }

        public void ShouldExitWithCode(int exitCode)
        {
            ShouldExit = true;
            ExitCode = exitCode;
        }
    }
}