using System.Collections;
using System.Collections.Generic;
using CommandDotNet.TestTools.Scenarios;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public class Scenarios : IEnumerable<IScenario>
    {
        private readonly List<IScenario> _scenarios = new List<IScenario>();

        public void Add(IScenario scenario)
        {
            _scenarios.Add(scenario);
        }

        public IEnumerator<IScenario> GetEnumerator() =>
            _scenarios.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}