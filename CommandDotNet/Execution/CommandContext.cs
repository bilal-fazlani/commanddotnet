using CommandDotNet.Parsing;
using CommandDotNet.Rendering;

namespace CommandDotNet.Execution
{
    public class CommandContext
    {
        public OriginalInput Original { get; }

        public TokenCollection Tokens { get; set; }

        public ICommand RootCommand { get; set; }

        public ParseResult ParseResult { get; set; }
        
        public ExecutionConfig ExecutionConfig { get; }

        public InvocationContext InvocationContext { get; } = new InvocationContext();

        public AppSettings AppSettings { get; }

        public IConsole Console => AppSettings.Console;

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
    }
}