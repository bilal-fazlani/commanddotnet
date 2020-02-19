using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class VersionOptionTests : TestBase
    {
        public VersionOptionTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void WhenVersionEnabled_BasicHelp_IncludesVersionOption()
        {
            var scenario = new Scenario
            {
                WhenArgs = "-h",
                Then = {ResultsContainsTexts = {"-v | --version  Show version information"}}
            };

            new AppRunner<App>(TestAppSettings.BasicHelp)
                .UseVersionMiddleware()
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void WhenVersionEnabled_DetailedHelp_IncludesVersionOption()
        {
            var scenario = new Scenario
            {
                WhenArgs = "-h",
                Then = {
                    ResultsContainsTexts = { @"  -v | --version          
  Show version information" }
                }
            };

            new AppRunner<App>(TestAppSettings.DetailedHelp)
                .UseVersionMiddleware()
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void WhenVersionDisabled_BasicHelp_DoesNotIncludeVersionOption()
        {
            new AppRunner<App>(TestAppSettings.BasicHelp)
                .VerifyScenario(TestOutputHelper, new Scenario
                {
                    WhenArgs = "-h",
                    Then = { ResultsNotContainsTexts = { "-v | --version" } }
                });
        }

        [Fact]
        public void WhenVersionDisabled_DetailedHelp_DoesNotIncludeVersionOption()
        {
            new AppRunner<App>(TestAppSettings.DetailedHelp)
                .VerifyScenario(TestOutputHelper, new Scenario
                {
                    WhenArgs = "-h",
                    Then = {ResultsNotContainsTexts = {"-v | --version"}}
                });
        }

        [Fact]
        public void WhenVersionEnabled_Version_LongName_OutputsVersion()
        {
            var scenario = new Scenario
            {
                WhenArgs = "--version",
                Then =
                {
                    Result = @"testhost.dll
16.2.0"
                }
            };

            new AppRunner<App>()
                .UseVersionMiddleware()
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void WhenVersionEnabled_Version_ShortName_OutputsVersion()
        {
            var scenario = new Scenario
            {
                WhenArgs = "-v",
                Then =
                {
                    Result = @"testhost.dll
16.2.0"
                }
            };
            
            new AppRunner<App>()
                .UseVersionMiddleware()
                .VerifyScenario(TestOutputHelper, scenario);
        }

        [Fact]
        public void WhenVersionDisabled_Version_LongName_NotRecognized()
        {
            new AppRunner<App>()
                .VerifyScenario(TestOutputHelper, new Scenario
                {
                    WhenArgs = "--version",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Unrecognized option '--version'" }
                    }
                });
        }

        [Fact]
        public void WhenVersionDisabled_Version_ShortName_NotRecognized()
        {
            new AppRunner<App>()
                .VerifyScenario(TestOutputHelper, new Scenario
                {
                    WhenArgs = "-v",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = {"Unrecognized option '-v'"}
                    }
                });
        }

        public class App
        {

        }
    }
}