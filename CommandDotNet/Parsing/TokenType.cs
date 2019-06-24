using System;

namespace CommandDotNet.Parsing
{
    public enum TokenType
    {
        Directive = 0,
        Option = 1,
        Value = 2,
        [Obsolete("Use Value instead.  it could belong to an Option")]
        Operand = 2,
        Separator = 3
    }
}