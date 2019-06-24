namespace CommandDotNet
{
    public interface IOption : IArgument
    {
        string Template { get; set; }
        string ShortName { get; set; }
        string SymbolName { get; set; }
        bool Inherited { get; set; }

        /// <summary>True when option is help or version</summary>
        bool IsSystemOption { get; set; }
    }
}