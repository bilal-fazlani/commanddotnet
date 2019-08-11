using CommandDotNet.Execution;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CommandContextForCommandMethods : TestBase
    {
        public CommandContextForCommandMethods(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
        
        [Fact]
        public void CommandContextIsNotIncludedInBasicHelp()
        {
            new AppRunner<App>(TestAppSettings.BasicHelp)
                .VerifyScenario(TestOutputHelper, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then = { Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  intOperand

Options:
  --stringOption" }
                });
        }

        [Fact]
        public void CommandContextIsNotIncludedInDetailedHelp()
        {
            new AppRunner<App>(TestAppSettings.DetailedHelp)
                .VerifyScenario(TestOutputHelper, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then = { Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  intOperand  <NUMBER>

Options:

  --stringOption  <TEXT>" }
                });
        }

        [Fact]
        public void CommandContextIsPassedToMethodParameter()
        {
            new AppRunner<App>().VerifyScenario(TestOutputHelper, new Scenario
            {
                WhenArgs = "Do 7 --stringOption optValue",
                Then =
                {
                    AllowUnspecifiedOutputs = true,
                    Outputs =
                    {
                        new App.DoResults
                        {
                            CommandContextIsNotNull = true,
                            IntOperand = 7,
                            StringOption = "optValue"
                        }
                    }
                }
            });
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do(CommandContext commandContext, [Operand] int intOperand, [Option] string stringOption = null)
            {
                TestOutputs.Capture(new DoResults
                {
                    CommandContextIsNotNull = commandContext != null,
                    IntOperand = intOperand,
                    StringOption = stringOption
                });
            }

            public class DoResults
            {
                public bool CommandContextIsNotNull;
                public int IntOperand;
                public string StringOption;
            }
        }
    }
}