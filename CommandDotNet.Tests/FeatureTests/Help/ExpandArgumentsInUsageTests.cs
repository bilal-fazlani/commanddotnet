using System.IO;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class ExpandArgumentsInUsageTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ExpandArgumentsInUsageTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Given_ExpandArgumentsInUsage_Then_ArgumentsAreListedByName()
        {
            new AppRunner<App>(new AppSettings { Help = {ExpandArgumentsInUsage = true}})
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then = { ResultsContainsTexts = { "Usage: dotnet testhost.dll Do <arg1> <arg2> [<optional>]" } }
                });
        }

        [Fact]
        public void Given_ExpandArgumentsInUsage_Then_ArgumentsAreListedAfterOptions()
        {
            new AppRunner<App>(new AppSettings { Help = { ExpandArgumentsInUsage = true } })
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do2 -h",
                    Then = { ResultsContainsTexts = { "Usage: dotnet testhost.dll Do2 [options] <arg1> <arg2> [<optional>]" } }
                });
        }

        public class App
        {
            public void Do(string arg1, FileInfo arg2, string optional = "lala")
            {

            }

            public void Do2(
                string arg1, FileInfo arg2, [Option] string option1, 
                string optional = "lala", [Option] string optionalOption = "fishies")
            {

            }
        }
    }
}