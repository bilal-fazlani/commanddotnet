using System;
using CommandDotNet.Exceptions;

namespace CommandDotNet.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            if(BooleanMode == BooleanMode.Unknown)
                throw new AppRunnerException("BooleanMode can not be set to BooleanMode.Unknown explicitly");
        }
        public bool ShowArgumentDetails { get; set; } = true;

        public BooleanMode BooleanMode { get; set; } = BooleanMode.Implicit;

        public bool ThrowOnUnexpectedArgument { get; set; } = true;
        
        public bool AllowArgumentSeparator { get; set; }

        public ArgumentMode MethodArgumentMode { get; set; } = ArgumentMode.Parameter;
    }

    public enum ArgumentMode
    {
        Parameter = 0,
        Option = 1
    }
}