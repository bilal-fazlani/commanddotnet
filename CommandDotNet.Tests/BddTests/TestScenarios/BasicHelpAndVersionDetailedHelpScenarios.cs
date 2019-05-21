using CommandDotNet.Tests.BddTests.Apps;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class BasicHelpAndVersionDetailedHelpScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<NoCommandApp>("no args and no default method - show help")
                {
                    WhenArgs = null,
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information"
                    }
                },
                new Given<NoCommandApp>("--help shows help - no commands")
                {
                    WhenArgs = "--help",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information"
                    }
                },
                new Given<NoCommandApp>("-h shows help - no commands")
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information"
                    }
                },
                new Given<NoCommandApp>("--version shows version")
                {
                    WhenArgs = "--version",
                    Then =
                    {
                        Help = @"testhost.dll
15.9.0"
                    }
                },
                new Given<NoCommandApp>("-v shows version")
                {
                    WhenArgs = "--version",
                    Then =
                    {
                        Help = @"testhost.dll
15.9.0"
                    }
                }
            };
    }
}