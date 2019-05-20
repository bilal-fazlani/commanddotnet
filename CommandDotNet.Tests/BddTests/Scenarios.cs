using System.Collections;
using System.Collections.Generic;

namespace CommandDotNet.Tests.BddTests
{
    public class Scenarios : IEnumerable<IScenario>
    {
        private List<IScenario> _scenarios = new List<IScenario>();

        public void Add(IScenario scenario)
        {
            this._scenarios.Add(scenario);
        }

        IEnumerator<IScenario> IEnumerable<IScenario>.GetEnumerator() =>
            this._scenarios.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this._scenarios.GetEnumerator();
    }
}