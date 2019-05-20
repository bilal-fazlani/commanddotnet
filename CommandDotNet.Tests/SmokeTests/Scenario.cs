using System;

namespace CommandDotNet.Tests.SmokeTests
{
    public class Scenario<T> : IScenario
    {
        public string Name { get; }
        public virtual Type AppType => typeof(T);
        public string Args { get; set; }

        public int? ExitCode { get; set; }
        public string Help { get; set; }

        public Scenario(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return $"{AppType.Name}: {Name}";
        }
    }
}