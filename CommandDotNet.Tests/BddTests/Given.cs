using System;

namespace CommandDotNet.Tests.BddTests
{
    public class Given<T> : IScenario
    {
        public string Name { get; }
        public Type GivenAppType => typeof(T);
        public string WhenArgs { get; set; }

        public ScenarioThen Then { get; } = new ScenarioThen();

        public Given(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return $"{this.GivenAppType.Name}: {this.Name}";
        }
    }
}