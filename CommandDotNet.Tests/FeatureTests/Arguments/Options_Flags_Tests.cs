using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_Flags_Tests
    {
        private readonly ITestOutputHelper _output;

        public Options_Flags_Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Help_DoesNotInclude_BoolTypeOrAllowedArgumentValues()
        {
            new AppRunner<FlagApp>().VerifyScenario(_output, new Scenario
            {
                // because the value should not be provided
                WhenArgs = "Do -h",
                Then = { Result = @"Usage: dotnet testhost.dll Do [options]

Options:

  --flag" }
            });
        }

        [Fact]
        public void WhenFlagIsSpecified_ValueIsTrue()
        {
            new AppRunner<FlagApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Do --flag",
                Then = { Outputs = { true } }
            });
        }

        [Fact]
        public void WhenFlagIsNotSpecified_ValueIsFalse()
        {
            new AppRunner<FlagApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Do",
                Then = { Outputs = { false } }
            });
        }

        [Fact]
        public void FlagsCanBeClubbed()
        {
            new AppRunner<FlagApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "Club -ab",
                Then = { Outputs = { new ClubResults { FlagA = true, FlagB = true } } }
            });
        }

        private class FlagApp
        {
            private TestOutputs TestOutputs { get; set; }

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

        private class ClubResults
        {
            public bool FlagA { get; set; }
            public bool FlagB { get; set; }
        }
    }
}