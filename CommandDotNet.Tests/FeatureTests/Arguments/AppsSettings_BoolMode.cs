using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class AppsSettings_BoolMode : ScenarioTestBase<AppsSettings_BoolMode>
    {
        private static AppSettings ImplicitBasicHelp = new AppSettings { BooleanMode = BooleanMode.Implicit, Help = { TextStyle = HelpTextStyle.Basic } };
        private static AppSettings ImplicitDetailedHelp = new AppSettings { BooleanMode = BooleanMode.Implicit, Help = { TextStyle = HelpTextStyle.Detailed } };
        private static AppSettings ExplicitBasicHelp = new AppSettings { BooleanMode = BooleanMode.Explicit, Help = { TextStyle = HelpTextStyle.Basic } };
        private static AppSettings ExplicitDetailedHelp = new AppSettings { BooleanMode = BooleanMode.Explicit, Help = { TextStyle = HelpTextStyle.Detailed } };

        public AppsSettings_BoolMode(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<App>("Explicit - Basic Help - does not include allowed values")
                {
                    And = {AppSettings = ExplicitBasicHelp},
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  operand

Options:
  -h | --help  Show help information
  --option"
                    }
                },
                new Given<App>("Explicit - Detailed Help - does include allowed values")
                {
                    And = {AppSettings = ExplicitDetailedHelp},
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  operand    <BOOLEAN>
  Allowed values: true, false


Options:

  -h | --help
  Show help information

  --option       <BOOLEAN>
  Allowed values: true, false"
                    }
                },
                new Given<App>("Implicit - Basic Help - does not include allowed values")
                {
                    And = {AppSettings = ImplicitBasicHelp},
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  operand

Options:
  -h | --help  Show help information
  --option"
                    }
                },
                new Given<App>("Implicit - Detailed Help - does not include allowed values for option")
                {
                    And = {AppSettings = ImplicitDetailedHelp},
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  operand    <BOOLEAN>
  Allowed values: true, false


Options:

  -h | --help
  Show help information

  --option"
                    }
                },
                new Given<App>("Implicit - exec - option is false if not specified")
                {
                    And = {AppSettings = ImplicitBasicHelp},
                    // bool value 'true' is operand
                    WhenArgs = "Do true",
                    Then = {Outputs = {new Result(false, true)}}
                },
                new Given<App>("Implicit - exec - option is true if specified")
                {
                    And = {AppSettings = ImplicitBasicHelp},
                    // bool value 'false' is operand
                    WhenArgs = "Do --option false",
                    Then = {Outputs = {new Result(true, false)}}
                },
                new Given<App>("Explicit - exec - option value must be the next argument")
                {
                    And = {AppSettings = ExplicitBasicHelp},
                    WhenArgs = "Do2 --option 2",
                    Then =
                    {
                        ExitCode = 2,
                        ResultsContainsTexts = { "'2' is not a valid Boolean" }
                    }
                },
                new Given<App>("Explicit - exec - option value is required")
                {
                    And = {AppSettings = ExplicitBasicHelp},
                    WhenArgs = "Do --option",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Missing value for option 'option'" }
                    }
                },
                new Given<App>("Explicit - exec - specified option value is used")
                {
                    And = {AppSettings = ExplicitBasicHelp},
                    WhenArgs = "Do --option false true",
                    Then = {Outputs = {new Result(false, true)}}
                },
            };

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do(
                [Option] bool option, 
                [Argument] bool operand)
            {
                TestOutputs.Capture(new Result(option, operand));
            }

            public void Do2(
                [Option] bool option,
                [Argument] int number)
            {
                TestOutputs.Capture(new Result(option, number));
            }
        }

        public class Result
        {
            public bool Option { get; set; }
            public bool Operand { get; set; }
            public int Number { get; set; }

            public Result(bool option, bool operand)
            {
                Option = option;
                Operand = operand;
            }

            public Result(bool option, int number)
            {
                Option = option;
                Number = number;
            }
        }
    }
}