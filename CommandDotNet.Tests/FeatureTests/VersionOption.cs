using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class VersionOption : TestBase
    {
        private static AppSettings VersionEnabledBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.EnableVersionOption = true);
        private static AppSettings VersionEnabledDetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.EnableVersionOption = true);
        private static AppSettings VersionDisabledBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.EnableVersionOption = false);
        private static AppSettings VersionEnabled = TestAppSettings.TestDefault.Clone(a => a.EnableVersionOption = true);

        public VersionOption(ITestOutputHelper output) : base(output)
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

            new AppRunner<App>(VersionEnabledBasicHelp)
                .UseBackwardsCompatibilityMode()
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

            new AppRunner<App>(VersionEnabledDetailedHelp)
                .UseBackwardsCompatibilityMode()
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
15.9.0"
                }
            };

            new AppRunner<App>()
                .UseVersionMiddleware()
                .VerifyScenario(TestOutputHelper, scenario);

            new AppRunner<App>(VersionEnabled)
                .UseBackwardsCompatibilityMode()
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
15.9.0"
                }
            };
            
            new AppRunner<App>()
                .UseVersionMiddleware()
                .VerifyScenario(TestOutputHelper, scenario);

            new AppRunner<App>(VersionEnabled)
                .UseBackwardsCompatibilityMode()
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