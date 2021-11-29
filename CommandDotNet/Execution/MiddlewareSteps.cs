namespace CommandDotNet.Execution
{
    public static class MiddlewareSteps
    {
        /// <summary>Runs first in the <see cref="MiddlewareStages.PreTokenize"/> stage</summary>
        public static MiddlewareStep DebugDirective { get; } = new(MiddlewareStages.PreTokenize);

        /// <summary>
        /// Runs early in the <see cref="MiddlewareStages.PreTokenize"/> stage after <see cref="DebugDirective"/>
        /// </summary>
        public static MiddlewareStep CancellationHandler { get; } = DebugDirective + 1000;

        /// <summary>
        /// Runs early in the <see cref="MiddlewareStages.PreTokenize"/> stage after <see cref="CancellationHandler"/>
        /// </summary>
        public static MiddlewareStep OnRunCompleted { get; } = CancellationHandler + 1000;

        public static class DependencyResolver
        {
            /// <summary>
            /// Runs early in the <see cref="MiddlewareStages.PreTokenize"/> stage after <see cref="OnRunCompleted"/>
            /// </summary>
            public static MiddlewareStep BeginScope { get; } = OnRunCompleted + 1000;
        }

        public static MiddlewareStep ParseDirective { get; } = Help.PrintHelpOnExit + 1000;

        public static MiddlewareStep Tokenize { get; } =
            new(MiddlewareStages.Tokenize, 0);

        /// <summary>
        /// Runs late in the <see cref="MiddlewareStages.Tokenize"/> stage
        /// </summary>
        public static MiddlewareStep CreateRootCommand { get; } =
            new(MiddlewareStages.Tokenize, short.MaxValue - 1000);

        public static MiddlewareStep ParseInput { get; } =
            new(MiddlewareStages.ParseInput, 0);

        /// <summary>
        /// Runs after <see cref="ParseInput"/> to respond to parse errors
        /// </summary>
        public static MiddlewareStep TypoSuggest { get; } = ParseInput + 1000;

        public static MiddlewareStep AssembleInvocationPipeline { get; } = ParseInput + 2000;

        /// <summary>Runs before <see cref="Help"/> to ensure default values are included in the help output</summary>
        public static MiddlewareStep Version { get; } = Help.CheckIfShouldShowHelp - 2000;

        /// <summary>Runs before <see cref="Help"/> to ensure default values are included in the help output</summary>
        public static MiddlewareStep SetArgumentDefaults { get; } = Help.CheckIfShouldShowHelp - 1000;

        /// <summary>Runs at the end of the <see cref="MiddlewareStages.ParseInput"/> stage</summary>
        public static class Help
        {
            public static MiddlewareStep CheckIfShouldShowHelp { get; } =
                new(MiddlewareStages.ParseInput, short.MaxValue);
            public static MiddlewareStep PrintHelpOnExit { get; } = DependencyResolver.BeginScope + 1000;
        }

        public static MiddlewareStep PipedInput { get; } =
            new(MiddlewareStages.PostParseInputPreBindValues, 0);

        /// <summary>
        /// Runs late in the <see cref="MiddlewareStages.PostParseInputPreBindValues"/>
        /// stage to enable other middleware to populate arguments
        /// </summary>
        public static MiddlewareStep ValuePromptMissingArguments { get; } =
            new(MiddlewareStages.PostParseInputPreBindValues, short.MaxValue - 1000);

        public static MiddlewareStep BindValues { get; } =
            new(MiddlewareStages.BindValues, 0);

        /// <summary>Runs after the <see cref="BindValues"/> step</summary>
        public static MiddlewareStep ResolveCommandClasses { get; } = BindValues + 1000;

        public static MiddlewareStep ValidateArity { get; } =
            new(MiddlewareStages.PostBindValuesPreInvoke, 0);

        public static MiddlewareStep CommandLogger { get; } = new(MiddlewareStages.Invoke, 0);

        /// <summary>Runs last in the <see cref="MiddlewareStages.Invoke"/> stage</summary>
        public static MiddlewareStep InvokeCommand { get; } = new(MiddlewareStages.Invoke, short.MaxValue);
    }
}