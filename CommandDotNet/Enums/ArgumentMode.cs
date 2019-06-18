using System;

namespace CommandDotNet
{
    public enum ArgumentMode
    {
        Operand = 0,
        Option = 1,
        [Obsolete("Use Operand instead")]
        Parameter = 0,
    }
}