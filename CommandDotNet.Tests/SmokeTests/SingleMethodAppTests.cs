using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.SmokeTests
{
    public class SingleMethodAppTests
    {
        [Fact]
        public void HelpListsCommandsWhenNoCommandGiven()
        {
            var results = new AppRunner<TestHelpOutput>().RunInMem("--help");
            results.ExitCode.Should().Be(0);
            results.HelpShouldBe(@"Usage: dotnet testhost.dll [options] [command]

Options:

  -v | --version
  Show version information

  -h | --help
  Show help information


Commands:

  Add

Use ""dotnet testhost.dll [command] --help"" for more information about a command.");
        }

        [Fact]
        public void HelpListsArgsWhenCommandGiven()
        {
            var results = new AppRunner<TestHelpOutput>().RunInMem("Add", "--help");
            results.ExitCode.Should().Be(0);
            results.HelpShouldBe(@"Usage: dotnet testhost.dll Add [arguments] [options]

Arguments:

  x    <NUMBER>

  y    <NUMBER>


Options:

  -h | --help
  Show help information");
        }

        [Fact]
        public void MethodIsCalledWithExpectedValues()
        {
            var results = new AppRunner<TestHelpOutput>().RunInMem("Add", "2", "3");
            results.ExitCode.Should().Be(5);
            results.ConsoleOut.Should().BeEmpty();
        }

        public class TestHelpOutput
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
    }
}