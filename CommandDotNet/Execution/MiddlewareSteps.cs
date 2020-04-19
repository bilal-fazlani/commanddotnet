using System;

namespace CommandDotNet.Execution
{
    public static class MiddlewareSteps
    {
        /// <summary>Runs first in the <see cref="MiddlewareStages.PreTokenize"/> stage</summary>
        public static MiddlewareStep DebugDirective { get; } = 
            new MiddlewareStep(MiddlewareStages.PreTokenize, int.MinValue + 1000);

        /// <summary>
        /// Runs second after <see cref="DebugDirective"/>
        /// in the <see cref="MiddlewareStages.PreTokenize"/> stage.
        /// This will catch all exceptions in the pipeline stack.
        /// </summary>
        public static MiddlewareStep ErrorHandler { get; } = 
            new MiddlewareStep(MiddlewareStages.PreTokenize, DebugDirective.OrderWithinStage + 1000);

        public static MiddlewareStep OnRunCompleted { get; } =
            new MiddlewareStep(MiddlewareStages.PreTokenize, ErrorHandler.OrderWithinStage + 1000);

        public static class DependencyResolver
        {
            public static MiddlewareStep BeginScope { get; } =
                new MiddlewareStep(MiddlewareStages.PreTokenize, -10000);
        }

        public static MiddlewareStep Tokenize { get; } =
            new MiddlewareStep(MiddlewareStages.Tokenize, -1000);

        public static MiddlewareStep CreateRootCommand { get; } =
            new MiddlewareStep(MiddlewareStages.Tokenize, int.MaxValue - 1000);

        public static MiddlewareStep ParseInput { get; } =
            new MiddlewareStep(MiddlewareStages.ParseInput, 0);

        public static MiddlewareStep TypoSuggest { get; } =
            new MiddlewareStep(MiddlewareStages.ParseInput, ParseInput.OrderWithinStage + 100);

        public static MiddlewareStep AssembleInvocationPipeline { get; } =
            new MiddlewareStep(MiddlewareStages.ParseInput, ParseInput.OrderWithinStage + 1000);

        /// <summary>Runs before <see cref="Help"/> to ensure default values are included in the help output</summary>
        public static MiddlewareStep Version { get; } =
            new MiddlewareStep(Help.CheckIfShouldShowHelp.Stage, Help.CheckIfShouldShowHelp.OrderWithinStage - 2000);

        /// <summary>Runs before <see cref="Help"/> to ensure default values are included in the help output</summary>
        public static MiddlewareStep SetArgumentDefaults { get; } =
            new MiddlewareStep(Help.CheckIfShouldShowHelp.Stage, Help.CheckIfShouldShowHelp.OrderWithinStage - 1000);

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
            new MiddlewareStep(MiddlewareStages.PostParseInputPreBindValues, -1);

        /// <summary>
        /// Runs late in the <see cref="MiddlewareStages.PostParseInputPreBindValues"/>
        /// stage to enable other middleware to populate arguments
        /// </summary>
        public static MiddlewareStep ValuePromptMissingArguments { get; } =
            new MiddlewareStep(MiddlewareStages.PostParseInputPreBindValues, int.MaxValue - 100);

        public static MiddlewareStep BindValues { get; } =
            new MiddlewareStep(MiddlewareStages.BindValues, 0);

        public static MiddlewareStep ResolveCommandClasses { get; } =
            new MiddlewareStep(MiddlewareStages.BindValues, BindValues.OrderWithinStage + 100);
    }
}