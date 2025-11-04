using System;

namespace CommandDotNet.Tests.SourceGen.TestCommands;

/// <summary>
/// Simple test command to verify basic source generation
/// </summary>
public class Calculator
{
    public void Add(int x, int y)
    {
        Console.WriteLine($"Result: {x + y}");
    }

    public void Subtract(int x, int y)
    {
        Console.WriteLine($"Result: {x - y}");
    }

    public void Multiply(int x, int y)
    {
        Console.WriteLine($"Result: {x * y}");
    }
}
