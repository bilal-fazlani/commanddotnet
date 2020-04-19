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
            new AppRunner<FlagApp>().Verify(_output, new Scenario
            {
                // because the value should not be provided
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options]

Options:

  --flag
"
                }
            });
        }

        [Fact]
        public void WhenFlagIsSpecified_ValueIsTrue()
        {
            new AppRunner<FlagApp>().Verify(_output, new Scenario
            {
                When = {Args = "Do --flag"},
                Then = { Captured = { true } }
            });
        }

        [Fact]
        public void WhenFlagIsNotSpecified_ValueIsFalse()
        {
            new AppRunner<FlagApp>().Verify(_output, new Scenario
            {
                When = {Args = "Do"},
                Then = { Captured = { false } }
            });
        }

        [Fact]
        public void FlagsCanBeClubbed()
        {
            new AppRunner<FlagApp>().Verify(_output, new Scenario
            {
                When = {Args = "Club -ab"},
                Then = { Captured = { new ClubResults { FlagA = true, FlagB = true } } }
            });
        }

        private class FlagApp
        {
            private TestCaptures TestCaptures { get; set; }

            public void Do([Option] bool flag)
            {
                TestCaptures.Capture(flag);
            }

            public void Club(
                [Option(ShortName = "a")] bool flagA, 
                [Option(ShortName = "b")] bool flagB)
            {
                TestCaptures.Capture(new ClubResults{FlagA = flagA, FlagB = flagB});
            }
        }

        private class ClubResults
        {
            public bool FlagA { get; set; }
            public bool FlagB { get; set; }
        }
    }
}
