using System;
using System.Collections.Generic;

namespace CommandDotNet.TestTools.Scenarios
{
    public class ScenarioThen
    {
        /// <summary>If specified, asserts the actual exit code is this value</summary>
        public int? ExitCode { get; set; }

        /// <summary>If specified, asserts the entire console output matches this value</summary>
        public string Output { get; set; }

        /// <summary>When specified, asserts the given text segments are contained in the <see cref="Output"/></summary>
        public List<string> OutputContainsTexts { get; set; } = new List<string>();

        /// <summary>When specified, asserts the given text segments are not contained in the <see cref="Output"/></summary>
        public List<string> OutputNotContainsTexts { get; set; } = new List<string>();

        /// <summary>
        /// When true, then <see cref="Captured"/> does not need to contain
        /// all of the outputs captured in the <see cref="TestCaptures"/>
        /// </summary>
        public bool AllowUnspecifiedCaptures { get; set; }

        /// <summary>
        /// If specified, asserts all of the outputs are captured in the <see cref="TestCaptures"/>.
        /// And asserts no additional outputs were captured unless <see cref="AllowUnspecifiedCaptures"/> is true.
        /// </summary>
        public List<object> Captured { get; set; } = new List<object>();

        /// <summary>
        /// A delegate to perform assertions on the <seealso cref="TestConsole.AllText"/>
        /// which includes the standard and error output streams.
        /// </summary>
        public Action<string> AssertOutput { get; set; }

        /// <summary>
        /// A delegate to perform assertions on the <see cref="CommandContext"/>
        /// </summary>
        public Action<CommandContext> AssertContext { get; set; }
    }
}