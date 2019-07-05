using System;
using System.IO;
using CommandDotNet.Help;
using CommandDotNet.Invocation;
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

        public bool EnableVersionOption { get; set; } = true;

        public bool EnableDirectives { get; set; }

        [Obsolete("Use correctly spelled PromptForArgumentsIfNotProvided")]
        public bool PrompForArgumentsIfNotProvided
        {
            get => PromptForArgumentsIfNotProvided;
            set => PromptForArgumentsIfNotProvided = value;
        }

        public bool PromptForArgumentsIfNotProvided { get; set; }

        public AppHelpSettings Help { get; set; } = new AppHelpSettings();

        [Obsolete("Use Help.TextStyle")]
        public HelpTextStyle HelpTextStyle
        {
            get => Help.TextStyle; 
            set => Help.TextStyle = value;
        }
        
        public ArgumentTypeDescriptors ArgumentTypeDescriptors { get; internal set; } = new ArgumentTypeDescriptors();
        
        internal IHelpProvider CustomHelpProvider { get; set; }

        internal ICommandInvoker CommandInvoker { get; set; } = new DefaultCommandInvoker();
        
        internal TextWriter Out { get; set; } = Console.Out;
        internal TextWriter Error { get; set; } = Console.Error;
    }
}