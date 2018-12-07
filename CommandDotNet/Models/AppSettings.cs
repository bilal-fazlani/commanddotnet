using CommandDotNet.CommandInvoker;
using CommandDotNet.Exceptions;
using CommandDotNet.HelpGeneration;

namespace CommandDotNet.Models
{
    public class AppSettings
    {
        private bool _enableCommandHooks;
        
        public AppSettings()
        {
            if(BooleanMode == BooleanMode.Unknown)
                throw new AppRunnerException("BooleanMode can not be set to BooleanMode.Unknown explicitly");
            CommandInvoker = new DefaultCommandInvoker();
        }
        public BooleanMode BooleanMode { get; set; } = BooleanMode.Implicit;

        public bool ThrowOnUnexpectedArgument { get; set; } = true;
        
        public bool AllowArgumentSeparator { get; set; }

        public ArgumentMode MethodArgumentMode { get; set; } = ArgumentMode.Parameter;

        public Case Case { get; set; } = Case.DontChange;

        public bool EnableVersionOption { get; set; } = true;

        public bool EnableCommandHooks
        {
            get => _enableCommandHooks;
            set
            {
                _enableCommandHooks = value;
                if (value)
                {
                    CommandInvoker = new PrePostHookCommandInvoker(new DefaultCommandInvoker());
                }
                else
                {
                    CommandInvoker = new DefaultCommandInvoker();
                }
            }
        }

        public bool PrompForArgumentsIfNotProvided { get; set; }

        public HelpTextStyle HelpTextStyle { get; set; } = HelpTextStyle.Detailed;
        
        internal IHelpProvider CustomHelpProvider { get; set; }

        internal ICommandInvoker CommandInvoker { get; set; }
    }
}