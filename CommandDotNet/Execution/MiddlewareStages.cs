namespace CommandDotNet.Execution
{
    public enum MiddlewareStages
    {
        Configuration = 1000,
        Tokenize = Configuration + 1000,
        Parsing = Tokenize + 1000,
        Invocation = Parsing + 1000,
    }
}