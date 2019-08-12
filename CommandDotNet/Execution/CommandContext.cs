using CommandDotNet.Parsing;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;

namespace CommandDotNet.Execution
{
    public class CommandContext
    {
        /// <summary>The original string array and first parse of tokens</summary>
        public OriginalInput Original { get; }

        /// <summary>
        /// The current working tokens. This collection is update with
        /// <see cref="TokenTransformation"/>s during <see cref="MiddlewareStages.TransformTokens"/>.
        /// Modifications should be performed via <see cref="TokenTransformation"/>s so they
        /// can be logged with the Parse directive
        /// </summary>
        public TokenCollection Tokens { get; set; }

        /// <summary>
        /// The root command for the type specified in <see cref="AppRunner{TRootCommandType}"/>. 
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

        public AppConfig AppConfig { get; }

        public IConsole Console => AppConfig.Console;

        public IServices Services { get; } = new Services();

        public CommandContext(
            string[] originalArgs, 
            TokenCollection originalTokens,
            AppConfig appConfig)
        {
            Original = new OriginalInput(originalArgs, originalTokens);
            Tokens = originalTokens;
            AppConfig = appConfig;
        }
    }
}