using System;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public class Given<T> : IScenario
    {
        public string Name { get; }
        public Type AppType => typeof(T);

        public ScenarioAnd And { get; } = new ScenarioAnd();
        public string WhenArgs { get; set; }
        public ScenarioThen Then { get; } = new ScenarioThen();

        IScenarioContext IScenario.Context { get; set; }

        public string SkipReason { get; set; }
        public string NotSupportedReason { get; set; }

        public Given(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            var context = ((IScenario)this).Context;

            var name = SkipReason == null
                ? Name
                : $"{Name} ({SkipReason})";

            if (context == null)
            {
                return name;
            }

            return context.Description == null 
                ? $"{context.Host.GetType().Name} > {name}" 
                : $"{context.Host.GetType().Name} > {name} ({context.Description})";
        }
    }
}