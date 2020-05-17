namespace CommandDotNet.Parsing
{
    public interface IParseError
    {
        public string Message { get; }
        public Command Command { get; }
    }
}