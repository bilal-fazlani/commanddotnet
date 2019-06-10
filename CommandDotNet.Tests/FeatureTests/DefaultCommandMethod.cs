using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class DefaultCommandMethod : ScenarioTestBase<DefaultCommandMethod>
    {
        private static AppSettings BasicHelp = new AppSettings { Help = { TextStyle = HelpTextStyle.Basic }, EnableVersionOption = false };
        private static AppSettings DetailedHelp = new AppSettings { Help = { TextStyle = HelpTextStyle.Detailed }, EnableVersionOption = false };
        
        public DefaultCommandMethod(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<WithoutParamsApp>("without params - Basic Help - includes other commands")
                {
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [options] [command]

Options:
  -h | --help  Show help information

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<WithoutParamsApp>("without params - Detailed Help - includes other commands")
                {
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [options] [command]

Options:

  -h | --help
  Show help information


Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<WithParamsApp>("with params - Basic Help - includes args and other commands")
                {
                    SkipReason = "Known Issue #24 - should include method args",
                    And = {AppSettings = BasicHelp},
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [options] [command]

Options:
  -h | --help     Show help information
  --text

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<WithParamsApp>("with params - Detailed Help - includes args and other commands")
                {
                    SkipReason = "Known Issue #24 - should include method args",
                    And = {AppSettings = DetailedHelp},
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [options] [command]

Options:

  -h | --help
  Show help information

  --text


Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<WithoutParamsApp>("without params - execute")
                {
                    WhenArgs = "",
                    Then =
                    {
                        Outputs = { WithoutParamsApp.DefaultMethodExecuted }
                    }
                },
                new Given<WithParamsApp>("with params - execute")
                {
                    SkipReason = "Known Issue #24",
                    WhenArgs = "abcde",
                    Then =
                    {
                        Outputs = { "abcde" }
                    }
                }
            };

        public class WithoutParamsApp
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

        public class WithParamsApp
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