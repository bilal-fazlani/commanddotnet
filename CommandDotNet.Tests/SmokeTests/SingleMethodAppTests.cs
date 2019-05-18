using CommandDotNet.Attributes;
using CommandDotNet.Tests.Utils;
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
            results.ExitCode.Should().Be(0);
            
            results.ConsoleOut.Should().Be("2+3=5");
            
            var inputs = results.Inputs.Get<TestHelpOutput>();
            inputs.X.Should().Be(2);
            inputs.Y.Should().Be(3);
        }

        public class TestHelpOutput
        {
            [InjectProperty]
            public TestWriter Writer { get; set; }
            
            [InjectProperty]
            public Inputs Inputs { get; set; }
            
            public int X { get; private set; }
            public int Y { get; private set; }
            
            public void Add(int x, int y)
            {
                Writer.Write($"{x}+{y}={x+y}");

                X = x;
                Y = y;
                Inputs.Capture(this);
            }
        }
    }
}