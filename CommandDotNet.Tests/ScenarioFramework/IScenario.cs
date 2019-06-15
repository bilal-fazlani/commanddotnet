using System;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public interface IScenario
    {
        Type AppType { get; }
        IScenarioContext Context { get; set; }
        ScenarioAnd And { get; }
        string WhenArgs { get; }
        string[] WhenArgsArray { get; }
        ScenarioThen Then { get; }
    }
}