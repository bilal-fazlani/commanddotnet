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

        public bool ThrowOnUnexpectedArgument { get; set; } = true;
        
        public bool AllowArgumentSeparator { get; set; }

        public ArgumentMode MethodArgumentMode { get; set; } = ArgumentMode.Operand;

        public Case Case { get; set; } = Case.DontChange;

        [Obsolete("Use appRunner.UseVersionMiddleware() extension method")]
        public bool EnableVersionOption { get; set; } = true;

        /// <summary>
        /// Set to true to tokenize arguments as directives,
        /// captured in <see cref="CommandContext.Tokens"/>.Directives
        /// for use by middleware
        /// </summary>
        public bool EnableDirectives { get; set; }

        [Obsolete("Use appRunner.UsePromptForMissingOperands() extension method")]
        public bool PrompForArgumentsIfNotProvided
        {
            get => PromptForMissingOperands;
            set => PromptForMissingOperands = value;
        }

        [Obsolete("Use appRunner.UsePromptForMissingOperands() extension method")]
        public bool PromptForMissingOperands { get; set; }

        public AppHelpSettings Help { get; set; } = new AppHelpSettings();

        [Obsolete("Use Help.TextStyle")]
        public HelpTextStyle HelpTextStyle
        {
            get => Help.TextStyle; 
            set => Help.TextStyle = value;
        }
        
        public ArgumentTypeDescriptors ArgumentTypeDescriptors { get; internal set; } = new ArgumentTypeDescriptors();
    }
}