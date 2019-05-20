using System;
using System.Collections;
using System.Collections.Generic;

namespace CommandDotNet.Tests.SmokeTests
{
    public class Scenario<T> : IScenario
    {
        public string Name { get; }
        public Type AppType => typeof(T);
        public string Args { get; set; }

        public int? ExitCode { get; set; }
        public string Help { get; set; }
        public IList Outputs { get; } = new List<object>();

        public Scenario(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{AppType.Name}: {Name}";
        }
    }
}