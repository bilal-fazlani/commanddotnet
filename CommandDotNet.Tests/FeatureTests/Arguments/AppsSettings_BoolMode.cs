using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class AppsSettings_BoolMode : TestBase
    {
        private static AppSettings ImplicitBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.BooleanMode = BooleanMode.Implicit);
        private static AppSettings ImplicitDetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.BooleanMode = BooleanMode.Implicit);

        private static AppSettings ExplicitBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.BooleanMode = BooleanMode.Explicit);
        private static AppSettings ExplicitDetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.BooleanMode = BooleanMode.Explicit);

        public AppsSettings_BoolMode(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void WhenExplicit_BasicHelp_DoesNotIncludeAllowedValues()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = ExplicitBasicHelp },
                WhenArgs = "Do -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  operand

Options:
  --option
  -h | --help  Show help information"
                }
            });
        }

        [Fact]
        public void WhenExplicit_DetailedHelp_IncludesAllowedValues()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = ExplicitDetailedHelp },
                WhenArgs = "Do -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  operand    <BOOLEAN>
  Allowed values: true, false


Options:

  --option       <BOOLEAN>
  Allowed values: true, false

  -h | --help
  Show help information"
                }
            });
        }

        [Fact]
        public void WhenImplicit_BasicHelp_DoesNotIncludeAllowedValues()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = ImplicitBasicHelp },
                WhenArgs = "Do -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  operand

Options:
  --option
  -h | --help  Show help information"
                }
            });
        }

        [Fact]
        public void WhenImplicit_DetailedHelp_DoesNotIncludeAllowedValuesForOption()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = ImplicitDetailedHelp },
                WhenArgs = "Do -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  operand    <BOOLEAN>
  Allowed values: true, false


Options:

  --option

  -h | --help
  Show help information"
                }
            });
        }

        [Fact]
        public void WhenImplicit_Exec_OptionsIsFalseIfNotSpecified()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = ImplicitBasicHelp },
                // bool value 'true' is operand
                WhenArgs = "Do true",
                Then = { Outputs = { new Result(false, true) } }
            });
        }

        [Fact]
        public void WhenImplicit_Exec_OptionsIsTrueIfSpecified()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = ImplicitBasicHelp },
                // bool value 'false' is operand
                WhenArgs = "Do --option false",
                Then = { Outputs = { new Result(true, false) } }
            });
        }

        [Fact]
        public void WhenExplicit_Exec_OptionValueMustFollowTheArgument()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = ExplicitBasicHelp },
                WhenArgs = "Do2 --option 2",
                Then =
                {
                    ExitCode = 2,
                    ResultsContainsTexts = { "'2' is not a valid Boolean" }
                }
            });
        }

        [Fact]
        public void WhenExplicit_Exec_OptionValueIsRequired()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = ExplicitBasicHelp },
                WhenArgs = "Do --option",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { "Missing value for option 'option'" }
                }
            });
        }

        [Fact]
        public void WhenExplicit_Exec_SpecifiedOptionValueIsUsed()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = ExplicitBasicHelp },
                WhenArgs = "Do --option false true",
                Then = { Outputs = { new Result(false, true) } }
            });
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do(
                [Option] bool option, 
                [Operand] bool operand)
            {
                TestOutputs.Capture(new Result(option, operand));
            }

            public void Do2(
                [Option] bool option,
                [Operand] int number)
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