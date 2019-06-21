using CommandDotNet.Attributes;

namespace CommandDotNet
{
    public interface IApplicationMetadata: INameAndDescription
    {
        string ExtendedHelpText { get; }
        string Syntax { get; }
    }
}