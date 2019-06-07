using CommandDotNet.Attributes;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_Flags : ScenarioTestBase<Options_Flags>
    {
        public Options_Flags(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<FlagApp>($"help does not include bool type or allowed arguments")
                {
                    // because the value should not be provided
                    WhenArgs = "Do -h",
                    Then = {Result = @"Usage: dotnet testhost.dll Do [options]

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
                new Given<FlagApp>($"clubbing is supported")
                {
                    WhenArgs = "Club -ab",
                    Then = {Outputs = {new ClubResults{FlagA = true, FlagB = true}}}
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

            public void Club(
                [Option(ShortName = "a")] bool flagA, 
                [Option(ShortName = "b")] bool flagB)
            {
                TestOutputs.Capture(new ClubResults{FlagA = flagA, FlagB = flagB});
            }
        }

        public class ClubResults
        {
            public bool FlagA { get; set; }
            public bool FlagB { get; set; }
        }
    }
}