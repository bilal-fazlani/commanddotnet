using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_Name_New_Tests
    {
        private readonly Option[] _options;

        public Options_Name_New_Tests(ITestOutputHelper testOutputHelper)
        {
            Command cmd = null;

            new AppRunner<App>(new AppSettings { LongNameAlwaysDefaultsToSymbolName = true })
                .CaptureState(ctx => cmd = ctx.ParseResult.TargetCommand, MiddlewareStages.PostParseInputPreBindValues, exitAfterCapture: true)
                .RunInMem("Do", testOutputHelper);
            _options = cmd.Options.ToArray();
        }

        [InlineData(0, "defaultName", null, "defaultName")]
        [InlineData(1, "longName1", null, "longName1")]
        [InlineData(2, "shortNameOverride", 'a', "shortNameOverride")]
        [InlineData(3, "longName2", 'b', "longName2")]
        [InlineData(4, null, 'c', "c")]
        [Theory]
        public void NameShouldBe(int i, string longName, char? shortName, string name)
        {
            var option = _options[i];
            option.LongName.Should().Be(longName);
            option.ShortName.Should().Be(shortName);
            option.Name.Should().Be(name);
        }

        class App
        {
            public int Do(
                [Option] string defaultName,
                [Option(LongName = "longName1")] string longNameOverride,
                [Option(ShortName = "a")] string shortNameOverride,
                [Option(ShortName = "b", LongName = "longName2")] string shortAndLongNameOverride,
                [Option(ShortName = "c", LongName = null)] string longNameNull,
                [Option(ShortName = "d", LongName = "")] string longNameEmpty)
            {
                return 1;
            }
        }
    }
}