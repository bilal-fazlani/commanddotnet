using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.SmokeTests
{
    public class EmptyAppTests
    {
        [Fact]
        public void NoInputDefaultsToHelp()
        {
            var results = new AppRunner<EmptyApp>().RunInMem();
            results.ExitCode.Should().Be(0);
            results.HelpShouldBe(@"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information");
        }

        [Fact]
        public void HelpOptionPrintsHelp()
        {
            var results = new AppRunner<EmptyApp>().RunInMem("--help");
            results.ExitCode.Should().Be(0);
            results.HelpShouldBe(@"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information");
        }

        public class EmptyApp
        {
            
        }
    }
}