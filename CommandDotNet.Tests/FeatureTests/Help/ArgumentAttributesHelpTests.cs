using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class ArgumentAttributesHelpTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ArgumentAttributesHelpTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void BasicHelp_Includes_Description()
        {
            new AppRunner<App>(TestAppSettings.BasicHelp)
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "Do -h",
                        Then =
                        {
                            Result = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:
  operand   operand-descr
  argument  argument-descr

Options:
  --option  option-descr"
                        }
                    });
        }

        [Fact]
        public void DetailedHelp_Includes_Description()
        {
            new AppRunner<App>(TestAppSettings.DetailedHelp)
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "Do -h",
                        Then =
                        {
                            Result = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:

  operand   <TEXT>
  operand-descr

  argument  <TEXT>
  argument-descr

Options:

  --option  <TEXT>
  option-descr"
                        }
                    });
        }

        public class App
        {
            public void Do(
                [Option(Description = "option-descr")] string option,
                [Operand(Description = "operand-descr")] string operand,
                [Argument(Description = "argument-descr")] string argument) { }
        }
    }
}