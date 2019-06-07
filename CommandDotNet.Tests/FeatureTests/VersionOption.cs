using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class VersionOption : ScenarioTestBase<VersionOption>
    {
        private static AppSettings VersionEnabledBasicHelp = new AppSettings { EnableVersionOption = true, Help = { TextStyle = HelpTextStyle.Basic } };
        private static AppSettings VersionEnabledDetailedHelp = new AppSettings { EnableVersionOption = true, Help = { TextStyle = HelpTextStyle.Detailed } };
        private static AppSettings VersionDisabled = new AppSettings { EnableVersionOption = false };

        public VersionOption(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<NoCommandApp>("when enabled, basic help for app includes version option")
                {
                    And = { AppSettings = VersionEnabledBasicHelp },
                    WhenArgs = "-h",
                    Then = { ResultsContainsTexts = { "-v | --version  Show version information" } }
                },
                new Given<NoCommandApp>("when enabled, detailed help for app includes version option")
                {
                    And = { AppSettings = VersionEnabledDetailedHelp },
                    WhenArgs = "-h",
                    Then = { 
                        ResultsContainsTexts = { @"  -v | --version          
  Show version information" }
                    }
                },
                new Given<NoCommandApp>("when disabled, basic help for app includes version option")
                {
                    And = { AppSettings = VersionDisabled },
                    WhenArgs = "-h",
                    Then = { ResultsNotContainsTexts = { "-v | --version  Show version information" } }
                },
                new Given<NoCommandApp>("when disabled, detailed help for app includes version option")
                {
                    And = { AppSettings = VersionDisabled },
                    WhenArgs = "-h",
                    Then = { ResultsNotContainsTexts = { "-v | --version  Show version information" } }
                },
                new Given<NoCommandApp>("when enabled, --version shows version")
                {
                    And = { AppSettings = VersionEnabledBasicHelp },
                    WhenArgs = "--version",
                    Then =
                    {
                        Result = @"testhost.dll
15.9.0"
                    }
                },
                new Given<NoCommandApp>("when enabled, -v shows version")
                {
                    And = { AppSettings = VersionEnabledDetailedHelp },
                    WhenArgs = "-v",
                    Then =
                    {
                        Result = @"testhost.dll
15.9.0"
                    }
                },
                new Given<NoCommandApp>("when disabled, --version not recognized")
                {
                    And = { AppSettings = VersionDisabled },
                    WhenArgs = "--version",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Unrecognized option '--version'" }
                    }
                },
                new Given<NoCommandApp>("when disabled, -v not recognized")
                {
                    And = { AppSettings = VersionDisabled },
                    WhenArgs = "-v",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Unrecognized option '-v'" }
                    }
                },
            };

        public class NoCommandApp
        {

        }
    }
}