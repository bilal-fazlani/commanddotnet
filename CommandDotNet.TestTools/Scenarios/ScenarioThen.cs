using System.Collections.Generic;

namespace CommandDotNet.TestTools.Scenarios
{
    public class ScenarioThen
    {
        public int? ExitCode { get; set; }
        public string Result { get; set; }
        public bool AllowUnspecifiedOutputs { get; set; }
        public List<object> Outputs { get; } = new List<object>();
        public List<string> ResultsContainsTexts { get; } = new List<string>();
        public List<string> ResultsNotContainsTexts { get; } = new List<string>();
    }
}