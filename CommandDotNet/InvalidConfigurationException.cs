using System;
using JetBrains.Annotations;

namespace CommandDotNet;

/// <summary>
/// <see cref="InvalidConfigurationException"/> indicates exceptions that can be fixed by developers.
/// These exceptions are not a result of user error.
/// </summary>
[PublicAPI]
public class InvalidConfigurationException : Exception
{
    public InvalidConfigurationException(string message) : base(message)
    {
    }

    public InvalidConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}