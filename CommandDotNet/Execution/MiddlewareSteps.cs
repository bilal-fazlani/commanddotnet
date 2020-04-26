using System;

namespace CommandDotNet.Execution
{
    public static class MiddlewareSteps
    {
        /// <summary>Runs first in the <see cref="MiddlewareStages.PreTokenize"/> stage</summary>
        public static MiddlewareStep DebugDirective { get; } = 
            new MiddlewareStep(MiddlewareStages.PreTokenize, int.MinValue + 1000);

        [Obsolete("This step is no longer used for appRunner.UseErrorHandler.")]
        public static MiddlewareStep ErrorHandler { get; } = DebugDirective + 100;

        /// <summary>
        /// Runs early in the <see cref="MiddlewareStages.PreTokenize"/> stage after <see cref="DebugDirective"/>
        /// </summary>
        public static MiddlewareStep OnRunCompleted { get; } = DebugDirective + 1000;

        public static class DependencyResolver
        {
            /// <summary>
            /// Runs early in the <see cref="MiddlewareStages.PreTokenize"/> stage after <see cref="OnRunCompleted"/>
            /// </summary>
            public static MiddlewareStep BeginScope { get; } = OnRunCompleted + 1000;
        }

        public static MiddlewareStep Tokenize { get; } =
            new MiddlewareStep(MiddlewareStages.Tokenize, 0);

        /// <summary>
        /// Runs late in the <see cref="MiddlewareStages.Tokenize"/> stage
        /// </summary>
        public static MiddlewareStep CreateRootCommand { get; } =
            new MiddlewareStep(MiddlewareStages.Tokenize, int.MaxValue - 1000);

        public static MiddlewareStep ParseInput { get; } =
            new MiddlewareStep(MiddlewareStages.ParseInput, 0);

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
                new MiddlewareStep(MiddlewareStages.ParseInput, int.MaxValue);
            public static MiddlewareStep PrintHelpOnExit { get; } =
                new MiddlewareStep(MiddlewareStages.PreTokenize, -10000);

            [Obsolete("use Help.CheckIfShouldShowHelp or Help.PrintHelpOnExit")]
            public static readonly MiddlewareStages Stage = CheckIfShouldShowHelp.Stage;

            [Obsolete("use Help.CheckIfShouldShowHelp or Help.PrintHelpOnExit")]
            public static readonly int Order = CheckIfShouldShowHelp.OrderWithinStage.GetValueOrDefault();

            [Obsolete("use Help.PrintHelpOnExit")]
            public static class PrintHelp
            {
                public static readonly MiddlewareStages Stage = MiddlewareStages.PreTokenize;
                public static readonly int Order = -10000;
            }
        }

        public static MiddlewareStep PipedInput { get; } =
            new MiddlewareStep(MiddlewareStages.PostParseInputPreBindValues, 0);

        /// <summary>
        /// Runs late in the <see cref="MiddlewareStages.PostParseInputPreBindValues"/>
        /// stage to enable other middleware to populate arguments
        /// </summary>
        public static MiddlewareStep ValuePromptMissingArguments { get; } =
            new MiddlewareStep(MiddlewareStages.PostParseInputPreBindValues, int.MaxValue - 1000);

        public static MiddlewareStep BindValues { get; } =
            new MiddlewareStep(MiddlewareStages.BindValues, 0);

        /// <summary>Runs after the <see cref="BindValues"/> step</summary>
        public static MiddlewareStep ResolveCommandClasses { get; } = BindValues + 1000;
    }
}