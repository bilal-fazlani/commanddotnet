namespace CommandDotNet;

public enum ArgumentMode
{
    /// <summary>aka: positional arguments</summary>
    Operand = 0,

    /// <summary>aka: named arguments</summary>
    Option = 1
}