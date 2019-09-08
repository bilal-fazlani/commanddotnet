using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class PrintHelpOptionTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PrintHelpOptionTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void BasicHelp_Includes_HelpOption()
        {
            var result = new AppRunner<App>(TestAppSettings.BasicHelp.Clone(s => s.Help.PrintHelpOption = true))
                .RunInMem("Do -h".SplitArgs(), _testOutputHelper);

            result.OutputShouldBe(@"Usage: dotnet testhost.dll Do [options]

Options:
  -h | --help  Show help information");
        }

        [Fact]
        public void DetailedHelp_Includes_HelpOption()
        {
            var result = new AppRunner<App>(TestAppSettings.DetailedHelp.Clone(s => s.Help.PrintHelpOption = true))
                .RunInMem("Do -h".SplitArgs(), _testOutputHelper);

            result.OutputShouldBe(@"Usage: dotnet testhost.dll Do [options]

Options:

  -h | --help
  Show help information");
        }

        public class App
        {
            public void Do() { }
        }
    }
}