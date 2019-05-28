using System.Collections;
using System.Collections.Generic;

namespace CommandDotNet.Tests.BddTests.Framework
{
    public class ScenarioThen
    {
        public int? ExitCode { get; set; }
        public string Help { get; set; }
        public bool OutputsStrict { get; set; }
        public List<object> Outputs { get; } = new List<object>();
        public List<string> HelpContainsTexts { get; } = new List<string>();
    }
}