using CommandDotNet.Parsing;
using CommandDotNet.Rendering;

namespace CommandDotNet.Execution
{
    public class CommandContext
    {
        /// <summary>The original string array and first parse of tokens</summary>
        public OriginalInput Original { get; }

        /// <summary>
        /// The current working tokens. This collection is update with
        /// input transformations during <see cref="MiddlewareStages.TransformInput"/>.
        /// Modifications should be performed via input transformations so they
        /// can be logged with the Parse directive
        /// </summary>
        public TokenCollection Tokens { get; set; }

        /// <summary>
        /// The root command for the type specified in <see cref="AppRunner{T}"/>. 
        /// This is populated in the <see cref="MiddlewareStages.Build"/> stage.
        /// </summary>
        public Command RootCommand { get; set; }

        /// <summary>
        /// The results of the <see cref="MiddlewareStages.ParseInput"/> stage.
        /// </summary>
        public ParseResult ParseResult { get; set; }

        /// <summary>
        /// This the invocation is partially populated in the <see cref="MiddlewareStages.ParseInput"/>
        /// stage and <see cref="Execution.InvocationContext.Instance"/> and <see cref="IInvocation.ParameterValues"/>
        /// are populated in the <see cref="MiddlewareStages.BindValues"/> stage.
        /// </summary>
        public InvocationContext InvocationContext { get; } = new InvocationContext();

        public ExecutionConfig ExecutionConfig { get; }

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