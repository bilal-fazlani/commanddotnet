using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class AppsSettingsBoolModeTests
    {
        private readonly ITestOutputHelper _output;
        private static readonly AppSettings ImplicitBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.BooleanMode = BooleanMode.Implicit);
        private static readonly AppSettings ImplicitDetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.BooleanMode = BooleanMode.Implicit);

        private static readonly AppSettings ExplicitBasicHelp = TestAppSettings.BasicHelp.Clone(a => a.BooleanMode = BooleanMode.Explicit);
        private static readonly AppSettings ExplicitDetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.BooleanMode = BooleanMode.Explicit);

        public AppsSettingsBoolModeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void WhenExplicit_BasicHelp_DoesNotIncludeAllowedValues()
        {
            new AppRunner<App>(ExplicitBasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:
  operand

Options:
  --option
"
                }
            });
        }

        [Fact]
        public void WhenExplicit_DetailedHelp_IncludesAllowedValues()
        {
            new AppRunner<App>(ExplicitDetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:

  operand  <BOOLEAN>
  Allowed values: true, false

Options:

  --option  <BOOLEAN>
  Allowed values: true, false
"
                }
            });
        }

        [Fact]
        public void WhenImplicit_BasicHelp_DoesNotIncludeAllowedValues()
        {
            new AppRunner<App>(ImplicitBasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:
  operand

Options:
  --option
"
                }
            });
        }

        [Fact]
        public void WhenImplicit_DetailedHelp_DoesNotIncludeAllowedValuesForOption()
        {
            new AppRunner<App>(ImplicitDetailedHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:

  operand  <BOOLEAN>
  Allowed values: true, false

Options:

  --option
"
                }
            });
        }

        [Fact]
        public void WhenImplicit_Exec_OptionsIsFalseIfNotSpecified()
        {
            new AppRunner<App>(ImplicitBasicHelp).Verify(_output, new Scenario
            {
                // bool value 'true' is operand
                WhenArgs = "Do true",
                Then = { Captured = { new Result(false, true) } }
            });
        }

        [Fact]
        public void WhenImplicit_Exec_OptionsIsTrueIfSpecified()
        {
            new AppRunner<App>(ImplicitBasicHelp).Verify(_output, new Scenario
            {
                // bool value 'false' is operand
                WhenArgs = "Do --option false",
                Then = { Captured = { new Result(true, false) } }
            });
        }

        [Fact]
        public void WhenExplicit_Exec_OptionValueMustFollowTheArgument()
        {
            new AppRunner<App>(ExplicitBasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do2 --option 2",
                Then =
                {
                    ExitCode = 2,
                    OutputContainsTexts = { "'2' is not a valid Boolean" }
                }
            });
        }

        [Fact]
        public void WhenExplicit_Exec_OptionValueIsRequired()
        {
            new AppRunner<App>(ExplicitBasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do --option",
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = { "Missing value for option 'option'" }
                }
            });
        }

        [Fact]
        public void WhenExplicit_Exec_SpecifiedOptionValueIsUsed()
        {
            new AppRunner<App>(ExplicitBasicHelp).Verify(_output, new Scenario
            {
                WhenArgs = "Do --option false true",
                Then = { Captured = { new Result(false, true) } }
            });
        }

        private class App
        {
            private TestCaptures TestCaptures { get; set; }

            public void Do(
                [Option] bool option, 
                [Operand] bool operand)
            {
                TestCaptures.Capture(new Result(option, operand));
            }

            public void Do2(
                [Option] bool option,
                [Operand] int number)
            {
                TestCaptures.Capture(new Result(option, number));
            }
        }

        private class Result
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