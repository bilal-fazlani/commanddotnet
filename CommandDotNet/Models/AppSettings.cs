using System;
using System.IO;
using CommandDotNet.CommandInvoker;
using CommandDotNet.Exceptions;
using CommandDotNet.HelpGeneration;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Models
{
    public class AppSettings
    {
        private BooleanMode _booleanMode = BooleanMode.Implicit;

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

        public ArgumentMode MethodArgumentMode { get; set; } = ArgumentMode.Parameter;

        public Case Case { get; set; } = Case.DontChange;

        public bool EnableVersionOption { get; set; } = true;
        
        public bool PrompForArgumentsIfNotProvided { get; set; }

        public HelpTextStyle HelpTextStyle { get; set; } = HelpTextStyle.Detailed;
        
        public ArgumentTypeDescriptors ArgumentTypeDescriptors { get; } = new ArgumentTypeDescriptors();
        
        internal IHelpProvider CustomHelpProvider { get; set; }

        internal ICommandInvoker CommandInvoker { get; set; } = new DefaultCommandInvoker();
        
        internal TextWriter Out { get; set; } = Console.Out;
        internal TextWriter Error { get; set; } = Console.Error;
    }
}