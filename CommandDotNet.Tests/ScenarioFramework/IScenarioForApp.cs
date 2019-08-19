using System;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public interface IScenarioForApp: IScenario
    {
        Type AppType { get; }
    }
}