using System;

namespace CommandDotNet.Tests.SourceGen.TestCommands;

/// <summary>
/// Test command using argument models
/// </summary>
public class ArgumentModelCommand
{
    public void Process(ProcessArgs args)
    {
        Console.WriteLine($"Name: {args.Name}, Count: {args.Count}, Verbose: {args.Verbose}");
    }
}

public class ProcessArgs : IArgumentModel
{
    [Operand]
    public string? Name { get; set; }

    [Option('c')]
    public int Count { get; set; } = 1;

    [Option('v')]
    public bool Verbose { get; set; }
}
