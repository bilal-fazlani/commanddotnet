using System;

namespace CommandDotNet.TestTools.Scenarios
{
    public class ScenarioGiven
    {
        public AppSettings AppSettings { get; set; }
        public Func<TestConsole, string> OnReadLine { get; set; }
        public string[] PipedInput { get; set; }
    }
}