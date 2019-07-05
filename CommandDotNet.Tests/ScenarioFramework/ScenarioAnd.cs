using System.Collections.Generic;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public class ScenarioAnd
    {
        public AppSettings AppSettings { get; set; }
        public List<object> Dependencies { get; set; } = new List<object>();
    }
}