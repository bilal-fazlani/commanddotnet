namespace CommandDotNet
{
    public interface IOption : IArgument
    {
        string Template { get; }
        string ShortName { get; }
        string SymbolName { get; }
        bool Inherited { get; }

        /// <summary>True when option is help or version</summary>
        bool IsSystemOption { get; }
    }
}