using System;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public interface IScenario
    {
        Type AppType { get; }
        IScenarioContext Context { get; set; }
        ScenarioAnd And { get; }
        string WhenArgs { get; }
        ScenarioThen Then { get; }
        string SkipReason { get; }
        string NotSupportedReason { get; }
    }
}