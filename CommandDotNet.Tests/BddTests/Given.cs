using System;
using CommandDotNet.Models;

namespace CommandDotNet.Tests.BddTests
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

        public Given(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            var context = ((IScenario)this).Context;
            return context?.Description == null 
                ? $"{AppType.Name}: {Name}" 
                : $"{AppType.Namespace}: {Name} ({context.Description})";
        }
    }
}