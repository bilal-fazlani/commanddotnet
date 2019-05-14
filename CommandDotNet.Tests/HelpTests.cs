using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class HelpTests : TestBase
    {
        public HelpTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void HelpListsCommandsWhenNoCommandGiven()
        {
            var results = new AppRunner<TestHelpOutput>().RunInMem("--help");
            results.ExitCode.Should().Be(0);

            var outLines = results.GetConsoleOutLines();

            outLines.Should().Contain(
                "Usage: dotnet testhost.dll [options] [command]",
                "Use \"dotnet testhost.dll [command] --help\" for more information about a command.",
                "Options:",
                "Commands:",
                "  Add");

            outLines.Should().NotContain(
                "Arguments:", 
                "  x    <NUMBER>",
                "  y    <NUMBER>");
        }

        [Fact]
        public void HelpListsArgsWhenCommandGiven()
        {
            var results = new AppRunner<TestHelpOutput>().RunInMem("Add", "--help");
            results.ExitCode.Should().Be(0);

            var outLines = results.GetConsoleOutLines();
            outLines.Should().Contain(
                "Usage: dotnet testhost.dll Add [arguments] [options]",
                "Arguments:",
                "  x    <NUMBER>",
                "  y    <NUMBER>",
                "Options:");
            
            outLines.Should().NotContain(
                "Commands:",
                "  Add");
        }

        public class TestHelpOutput
        {
            public void Add(int x, int y)
            {
                
            }
        }
    }
}