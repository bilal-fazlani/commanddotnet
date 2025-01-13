using System.Collections.Generic;
using CommandDotNet.Tokens;
using JetBrains.Annotations;

namespace CommandDotNet;

[PublicAPI]
public class OriginalInput(string[] args, TokenCollection tokens)
{
    /// <summary>The original string array passed to the program</summary>
    public IReadOnlyCollection<string> Args { get; } = args;

    /// <summary>The original tokens before any input transformations were applied</summary>
    public TokenCollection Tokens { get; } = tokens;
}