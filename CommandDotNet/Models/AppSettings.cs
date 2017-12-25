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
        public bool ShowParameterDetails { get; set; } = true;

        public BooleanMode BooleanMode { get; set; } = BooleanMode.Implicit;

        public bool ThrowOnUnexpectedArgument { get; set; } = true;
        
        public bool AllowArgumentSeparator { get; set; }
    }
}