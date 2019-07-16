using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class ArgumentAttributes
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ArgumentAttributes(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void BasicHelp_Includes_Description()
        {
            var result = new AppRunner<App>(TestAppSettings.BasicHelp)
                .RunInMem("Do -h".SplitArgs(), _testOutputHelper);

            result.OutputShouldBe(@"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:
  operand   operand-descr
  argument  argument-descr

Options:
  --option  option-descr");
        }

        [Fact]
        public void DetailedHelp_Includes_Description()
        {
            var result = new AppRunner<App>(TestAppSettings.DetailedHelp)
                .RunInMem("Do -h".SplitArgs(), _testOutputHelper);

            result.OutputShouldBe(@"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  operand     <TEXT>
  operand-descr

  argument    <TEXT>
  argument-descr


Options:

  --option    <TEXT>
  option-descr");
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