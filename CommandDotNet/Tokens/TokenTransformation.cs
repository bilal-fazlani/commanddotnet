using System;
using JetBrains.Annotations;

namespace CommandDotNet.Tokens;

[PublicAPI]
public class TokenTransformation(
    string name,
    int order,
    Func<CommandContext, TokenCollection, TokenCollection> transformation)
{
    public string Name { get; } = name;
    public int Order { get; } = order;
    public Func<CommandContext, TokenCollection, TokenCollection> Transformation { get; } = transformation;

    public override string ToString() => $"{nameof(TokenTransformation)}: {Name} ({Order})";
}