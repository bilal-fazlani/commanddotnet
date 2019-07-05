namespace CommandDotNet
{
    public interface IApplicationMetadata: INameAndDescription
    {
        string ExtendedHelpText { get; }
    }
}