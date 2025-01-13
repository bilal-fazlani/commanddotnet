using System;
using JetBrains.Annotations;

namespace CommandDotNet.Parsing;

/// <summary><see cref="ValueParsingException"/> indicates user error.</summary>
[PublicAPI]
public class ValueParsingException : Exception
{
    public ValueParsingException(string message) : base(message)
    {
    }

    public ValueParsingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}