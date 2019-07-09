namespace CommandDotNet.Execution
{
    public enum MiddlewareStages
    {
        Configuration = 100,
        Tokenize = Configuration + 100,
        Parsing = Tokenize + 100,
        Invocation = Parsing + 100,
    }
}