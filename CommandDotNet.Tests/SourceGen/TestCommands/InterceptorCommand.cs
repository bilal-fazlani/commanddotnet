using System;
using System.Threading.Tasks;
using CommandDotNet.Execution;

namespace CommandDotNet.Tests.SourceGen.TestCommands;

/// <summary>
/// Test command with interceptor to verify interceptor method detection and generation
/// </summary>
public class InterceptorCommand
{
    public Task<int> Intercept(CommandContext context, InterceptorExecutionDelegate next)
    {
        Console.WriteLine("[Before Command]");
        var result = next();
        Console.WriteLine("[After Command]");
        return result;
    }

    public void Execute(string message)
    {
        Console.WriteLine($"Message: {message}");
    }

    public void DoWork(int count)
    {
        Console.WriteLine($"Working {count} times");
    }
}
