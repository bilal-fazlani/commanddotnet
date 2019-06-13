using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class VersionOption : TestBase
    {
        private static AppSettings VersionEnabledBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.EnableVersionOption = true);
        private static AppSettings VersionEnabledDetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.EnableVersionOption = true);
        private static AppSettings VersionDisabled = new AppSettings { EnableVersionOption = false };

        public VersionOption(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void WhenVersionEnabled_BasicHelp_IncludesVersionOption()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = VersionEnabledBasicHelp },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "-v | --version  Show version information" } }
            });
        }

        [Fact]
        public void WhenVersionEnabled_DetailedHelp_IncludesVersionOption()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = VersionEnabledDetailedHelp },
                WhenArgs = "-h",
                Then = {
                    ResultsContainsTexts = { @"  -v | --version          
  Show version information" }
                }
            });
        }

        [Fact]
        public void WhenVersionDisabled_BasicHelp_IncludesVersionOption()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = VersionDisabled },
                WhenArgs = "-h",
                Then = { ResultsNotContainsTexts = { "-v | --version" } }
            });
        }

        [Fact]
        public void WhenVersionDisabled_DetailedHelp_IncludesVersionOption()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = VersionDisabled },
                WhenArgs = "-h",
                Then = { ResultsNotContainsTexts = { "-v | --version" } }
            });
        }

        [Fact]
        public void WhenVersionEnabled_Version_LongName_OutputsVersion()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = VersionEnabledBasicHelp },
                WhenArgs = "--version",
                Then =
                {
                    Result = @"testhost.dll
15.9.0"
                }
            });
        }

        [Fact]
        public void WhenVersionEnabled_Version_ShortName_OutputsVersion()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = VersionEnabledBasicHelp },
                WhenArgs = "-v",
                Then =
                {
                    Result = @"testhost.dll
15.9.0"
                }
            });
        }

        [Fact]
        public void WhenVersionEnabled_Version_LongName_NotRecognized()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = VersionDisabled },
                WhenArgs = "--version",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { "Unrecognized option '--version'" }
                }
            });
        }

        [Fact]
        public void WhenVersionEnabled_Version_ShortName_NotRecognized()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = VersionDisabled },
                WhenArgs = "-v",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { "Unrecognized option '-v'" }
                }
            });
        }

        public class App
        {

        }
    }
}