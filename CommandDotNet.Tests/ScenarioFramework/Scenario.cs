using System;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public class Scenario<T> : Scenario, IScenarioForApp
    {
        public Type AppType => typeof(T);

        public Scenario() : base()
        {
        }

        public Scenario(string name) : base(name)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}