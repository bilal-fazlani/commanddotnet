using System;
using System.Collections.Generic;

namespace CommandDotNet.TestTools.Scenarios
{
    public class ScenarioThen
    {
        /// <summary>If specified, asserts the actual exit code is this value</summary>
        public int? ExitCode { get; set; }

        /// <summary>If specified, asserts the entire console output matches this value</summary>
        public string Result { get; set; }

        /// <summary>
        /// When true, then <see cref="Outputs"/> does not need to contain
        /// all of the outputs captured in the <see cref="TestOutputs"/>
        /// </summary>
        public bool AllowUnspecifiedOutputs { get; set; }

        /// <summary>
        /// If specified, asserts all of the outputs are captured in the <see cref="TestOutputs"/>.
        /// And asserts no additional outputs were captured unless <see cref="AllowUnspecifiedOutputs"/> is true.
        /// </summary>
        public List<object> Outputs { get; set; } = new List<object>();

        /// <summary>When specified, asserts the given text segments are contained in the <see cref="Result"/></summary>
        public List<string> ResultsContainsTexts { get; set; } = new List<string>();

        /// <summary>When specified, asserts the given text segments are not contained in the <see cref="Result"/></summary>
        public List<string> ResultsNotContainsTexts { get; set; } = new List<string>();
    }
}