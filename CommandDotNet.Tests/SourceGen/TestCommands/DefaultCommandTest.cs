using System;

namespace CommandDotNet.Tests.SourceGen.TestCommands;

/// <summary>
/// Test command with default command to verify [DefaultCommand] detection
/// </summary>
public class DefaultCommandTest
{
    [DefaultCommand]
    public void RunDefault(string input)
    {
        Console.WriteLine($"Default: {input}");
    }

    public void Other(string value)
    {
        Console.WriteLine($"Other: {value}");
    }
}
