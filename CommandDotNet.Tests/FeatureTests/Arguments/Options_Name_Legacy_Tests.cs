using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_Name_Legacy_Tests
    {
        private readonly Option[] _options;

        public Options_Name_Legacy_Tests(ITestOutputHelper testOutputHelper)
        {
            Command cmd = null;
            new AppRunner<App>()
                .CaptureState(ctx => cmd = ctx.ParseResult.TargetCommand, MiddlewareStages.PostParseInputPreBindValues, exitAfterCapture: true)
                .RunInMem("Do", testOutputHelper);
            _options = cmd.Options.ToArray();
        }

        [Fact]
        public void GivenOptionAttr_DoesNotDefineAName_UsesParameterName()
        {
            _options[0].Name.Should().Be("defaultName");
            _options[0].ShortName.Should().BeNull();
        }

        [Fact]
        public void GivenOptionAttr_DefinesOnlyLongName_UsesLongName()
        {
            _options[1].Name.Should().Be("longname");
            _options[1].ShortName.Should().BeNull();
        }

        [Fact]
        public void GivenOptionAttr_DefinesOnlyShortName_UsesShortName()
        {
            _options[2].Name.Should().Be("a");
            _options[2].ShortName.Should().Be('a');
        }

        [Fact]
        public void GivenOptionAttr_DefinesLongAndShortName_UsesLongName()
        {
            _options[3].Name.Should().Be("blongname");
            _options[3].ShortName.Should().Be('b');
        }

        class App
        {
            public int Do(
                [Option] string defaultName,
                [Option(LongName = "longname")] string useLongNameOverride,
                [Option(ShortName = "a")] string useShortNameOnly,
                [Option(ShortName = "b", LongName = "blongname")] string useShortAndLongName)
            {
                return 1;
            }
        }
    }
}