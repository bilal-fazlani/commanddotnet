using System;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public interface IScenarioForApp: IScenario
    {
        Type AppType { get; }
    }
}