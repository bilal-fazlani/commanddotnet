namespace CommandDotNet
{
    public interface IApplicationMetadata
    {
        string Description { get; }
        string ExtendedHelpText { get; }
        string Syntax { get; }
    }
}