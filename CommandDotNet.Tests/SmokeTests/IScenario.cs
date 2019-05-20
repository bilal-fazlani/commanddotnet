using System;
using System.Collections;

namespace CommandDotNet.Tests.SmokeTests
{
    public interface IScenario
    {
        Type AppType { get; }
        string Args { get; }
        int? ExitCode { get; }
        string Help { get; }
        IList Outputs { get; }
    }
}