using System;

namespace CommandDotNet.Tests.BddTests
{
    public class Given<T> : IScenario
    {
        public string Name { get; }
        public Type AppType => typeof(T);
        public string SkipReason { get; set; }


        public string WhenArgs { get; set; }

        public ScenarioThen Then { get; } = new ScenarioThen();

        public Given(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{AppType.Name}: {Name}";
        }
    }
}