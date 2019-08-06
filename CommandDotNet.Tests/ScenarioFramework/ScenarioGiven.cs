using System;
using System.Collections.Generic;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public class ScenarioGiven
    {
        public AppSettings AppSettings { get; set; }
        public object[] Dependencies { get; set; }
        public Func<TestConsole, string> OnReadLine { get; set; }
        public string[] PipedInput { get; set; }
    }
}