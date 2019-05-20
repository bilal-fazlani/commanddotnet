using System;

namespace CommandDotNet.Tests.BddTests
{
    public interface IScenario
    {
        Type GivenAppType { get; }
        string WhenArgs { get; }
        ScenarioThen Then { get; }
    }
}