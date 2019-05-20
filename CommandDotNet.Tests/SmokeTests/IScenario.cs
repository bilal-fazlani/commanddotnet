using System;

namespace CommandDotNet.Tests.SmokeTests
{
    public interface IScenario
    {
        string Name { get; }
        Type AppType { get; }
        string Args { get; }
        int? ExitCode { get; }
        string Help { get; }
    }
}