using System;
using CommandDotNet.Execution;
using CommandDotNet.Help;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet
{
    public class AppSettings
    {
        private BooleanMode _booleanMode = BooleanMode.Implicit;

        /// <summary>
        /// When Explicit, options require a 'true' or 'false' value be specified.
        /// When Implicit, an option is considered false unless it's specified.
        /// The next argument will be considered a new argument.
        /// </summary>
        /// <remarks>
        /// BooleanMode applies to bool options only.
        /// </remarks>
        public BooleanMode BooleanMode
        {
            get => _booleanMode;
            set
            {
                if(value == BooleanMode.Unknown)
                    throw new AppRunnerException("BooleanMode can not be set to BooleanMode.Unknown explicitly");
                _booleanMode = value;
            }
        }

        /// <summary>
        /// When false, unexpected arguments will result in a parse failure with help message.<br/>
        /// When true, unexpected arguments will be ignored
        /// </summary>
        public bool IgnoreUnexpectedOperands { get; set; }

        /// <summary>
        /// When arguments are not decorated with [Operand] or [Option]
        /// DefaultArgumentMode is used to determine which mode to use.
        /// </summary>
        public ArgumentMode DefaultArgumentMode { get; set; } = ArgumentMode.Operand;

        /// <summary>The specified case is applied to command and argument names</summary>
        public Case Case { get; set; } = Case.DontChange;

        /// <summary>
        /// Set to true to tokenize arguments as directives,
        /// captured in <see cref="CommandContext.Tokens"/>.Directives
        /// for use by middleware
        /// </summary>
        public bool EnableDirectives { get; set; }

        /// <summary>Settings specific to built-in help providers</summary>
        public AppHelpSettings Help { get; set; } = new AppHelpSettings();

        /// <summary>
        /// The collection of <see cref="IArgumentTypeDescriptor"/>'s use to convert arguments
        /// from the commandline to the parameter & property types for the command methods.
        /// </summary>
        public ArgumentTypeDescriptors ArgumentTypeDescriptors { get; internal set; } = new ArgumentTypeDescriptors();

        #region Obsolete Members

        [Obsolete("Use DefaultArgumentMode instead")]
        public ArgumentMode MethodArgumentMode
        {
            get => DefaultArgumentMode;
            set => DefaultArgumentMode = value;
        }

        [Obsolete("Use IgnoreUnexpectedArguments instead")]
        public bool ThrowOnUnexpectedArgument
        {
            get => !IgnoreUnexpectedOperands;
            set => IgnoreUnexpectedOperands = !value;
        }

        [Obsolete("this is only used to display the arg separator in help. it does not make the separated arguments available for use.")]
        public bool AllowArgumentSeparator { get; set; }

        [Obsolete("Use appRunner.UseVersionMiddleware() extension method")]
        public bool EnableVersionOption { get; set; } = true;

        [Obsolete("Use appRunner.UsePromptForMissingOperands() extension method")]
        public bool PrompForArgumentsIfNotProvided
        {
            get => PromptForMissingOperands;
            set => PromptForMissingOperands = value;
        }

        [Obsolete("Use appRunner.UsePromptForMissingOperands() extension method")]
        public bool PromptForMissingOperands { get; set; }

        [Obsolete("Use Help.TextStyle")]
        public HelpTextStyle HelpTextStyle
        {
            get => Help.TextStyle;
            set => Help.TextStyle = value;
        }

        #endregion
    }
}