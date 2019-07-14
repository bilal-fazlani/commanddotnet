namespace CommandDotNet.Execution
{
    public enum MiddlewareStages
    {
        Configuration = 1000,
        Tokenize = Configuration + 1000,
        Building = Tokenize + 1000,
        Parsing = Building + 1000,
        Invocation = Parsing + 1000,
    }
}