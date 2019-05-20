using System;

namespace CommandDotNet.Tests.BddTests
{
    public interface IScenario
    {
        Type AppType { get; }
        string SkipReason { get; set; }
        string WhenArgs { get; }
        ScenarioThen Then { get; }
    }
}