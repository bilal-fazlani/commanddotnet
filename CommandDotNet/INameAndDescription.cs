namespace CommandDotNet;

public interface INameAndDescription
{
    string? Name { get; }
    string? Description { get; }
    string[]? DescriptionLines { get; }
}