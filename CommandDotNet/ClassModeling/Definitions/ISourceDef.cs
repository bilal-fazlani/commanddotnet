using CommandDotNet.Builders;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal interface ISourceDef : ICustomAttributesContainer
    {
        string Name { get; }
        string SourcePath { get; }
    }
}