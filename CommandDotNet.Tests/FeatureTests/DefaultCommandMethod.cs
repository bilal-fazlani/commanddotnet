using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class DefaultCommandMethod : ScenarioTestBase<DefaultCommandMethod>
    {
        private static AppSettings BasicHelp = new AppSettings { Help = { TextStyle = HelpTextStyle.Basic } };
        private static AppSettings DetailedHelp = new AppSettings { Help = { TextStyle = HelpTextStyle.Detailed } };
        
        public DefaultCommandMethod(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<DefaultCommandWithoutParamsApp>("default command method without params - Basic Help - includes other commands")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [options] [command]

Options:
  -v | --version  Show version information
  -h | --help     Show help information

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<DefaultCommandWithoutParamsApp>("default command method without params - Detailed Help - includes other commands")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [options] [command]

Options:

  -v | --version
  Show version information

  -h | --help
  Show help information


Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<DefaultCommandWithParams>("default command method with params - Basic Help - includes args and other commands")
                {
                    SkipReason = "Known Issue #24 - should include method args",
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [options] [command]

Options:
  -v | --version  Show version information
  -h | --help     Show help information

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<DefaultCommandWithParams>("default command method with params - Detailed Help - includes args and other commands")
                {
                    SkipReason = "Known Issue #24 - should include method args",
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [options] [command]

Options:

  -v | --version
  Show version information

  -h | --help
  Show help information


Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<DefaultCommandWithoutParamsApp>("default command method without params - execute")
                {
                    WhenArgs = "",
                    Then =
                    {
                        Outputs = { DefaultCommandWithoutParamsApp.DefaultMethodExecuted }
                    }
                },
                new Given<DefaultCommandWithParams>("default command method with params - execute")
                {
                    SkipReason = "Known Issue #24",
                    WhenArgs = "abcde",
                    Then =
                    {
                        Outputs = { "abcde" }
                    }
                }
            };

        public class DefaultCommandWithoutParamsApp
        {
            public const string DefaultMethodExecuted = "default executed";

            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            [DefaultMethod]
            public void DefaultMethod()
            {
                TestOutputs.Capture(DefaultMethodExecuted);
            }

            public void AnotherCommand()
            {
                TestOutputs.Capture(false);
            }
        }

        public class DefaultCommandWithParams
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            [DefaultMethod]
            public void DefaultMethod(
                [Argument(Description = "some text")]
                string text)
            {
                TestOutputs.Capture(text);
            }

            public void AnotherCommand()
            {
                TestOutputs.Capture(false);
            }
        }
    }
}