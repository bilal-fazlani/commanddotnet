using CommandDotNet.Attributes;
using CommandDotNet.Tests.BddTests.Framework;
using CommandDotNet.Tests.Utils;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class FlagScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<FlagApp>($"help is boolean")
                {
                    WhenArgs = "Do -h",
                    Then = {Help = @"Usage: dotnet testhost.dll Do [options]

Options:

  -h | --help
  Show help information

  --flag" }
                },
                new Given<FlagApp>($"when specified, flag is true")
                {
                    WhenArgs = "Do --flag",
                    Then = {Outputs = {true}}
                },
                new Given<FlagApp>($"when not specified, flag is false")
                {
                    WhenArgs = "Do",
                    Then = {Outputs = {false}}
                },
            };

        private class FlagApp
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do([Option] bool flag)
            {
                TestOutputs.Capture(flag);
            }
        }
    }
}