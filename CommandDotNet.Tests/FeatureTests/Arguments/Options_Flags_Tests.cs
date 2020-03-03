using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_Flags_Tests : TestBase
    {
        public Options_Flags_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Help_DoesNotInclude_BoolTypeOrAllowedArgumentValues()
        {
            Verify(new Scenario<FlagApp>
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
            Verify(new Scenario<FlagApp>
            {
                WhenArgs = "Do --flag",
                Then = { Outputs = { true } }
            });
        }

        [Fact]
        public void WhenFlagIsNotSpecified_ValueIsFalse()
        {
            Verify(new Scenario<FlagApp>
            {
                WhenArgs = "Do",
                Then = { Outputs = { false } }
            });
        }

        [Fact]
        public void FlagsCanBeClubbed()
        {
            Verify(new Scenario<FlagApp>
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

        public class ClubResults
        {
            public bool FlagA { get; set; }
            public bool FlagB { get; set; }
        }
    }
}