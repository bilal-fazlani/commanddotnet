using CommandDotNet.Tests.SmokeTests.Apps;

namespace CommandDotNet.Tests.SmokeTests.TestScenarios
{
    public class DefaultHelpAndVersionDetailedHelpScenarios: ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Scenario<NoCommandApp>("no args and no default method - show help")
                {
                    Args = null,
                    Help = @"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information"
                },
                new Scenario<NoCommandApp>("--help shows help - no commands")
                {
                    Args = "--help",
                    Help = @"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information"
                },
                new Scenario<NoCommandApp>("-h shows help - no commands")
                {
                    Args = "-h",
                    Help = @"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information"
                },
                new Scenario<NoCommandApp>("--version shows version")
                {
                    Args = "--version",
                    Help = @"testhost.dll
15.9.0"
                },
                new Scenario<NoCommandApp>("-v shows version")
                {
                    Args = "--version",
                    Help = @"testhost.dll
15.9.0"
                }
            };
    }
}