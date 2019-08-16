using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class GeneralHelpTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GeneralHelpTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void QuestionMark_ShowsHelp()
        {
            var result = new AppRunner<App>().RunInMem("-?".SplitArgs(), _testOutputHelper);
            result.OutputContains("Usage: dotnet testhost.dll [command]");
        }

        [Fact]
        public void ShortName_ShowsHelp()
        {
            var result = new AppRunner<App>().RunInMem("-h".SplitArgs(), _testOutputHelper);
            result.OutputContains("Usage: dotnet testhost.dll [command]");
        }

        [Fact]
        public void LongName_ShowsHelp()
        {
            var result = new AppRunner<App>().RunInMem("--help".SplitArgs(), _testOutputHelper);
            result.OutputContains("Usage: dotnet testhost.dll [command]");
        }

        public class App
        {
            public void Do(){ }
        }
    }
}