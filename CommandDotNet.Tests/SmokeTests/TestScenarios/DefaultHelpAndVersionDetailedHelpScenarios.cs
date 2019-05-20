using CommandDotNet.Tests.SmokeTests.Apps;

namespace CommandDotNet.Tests.SmokeTests.TestScenarios
{
    public class DefaultHelpAndVersionDetailedHelpScenarios: ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Scenario<EmptyApp>("no args and no default method - show help")
                {
                    Args = null,
                    Help = @"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information"
                },
                new Scenario<EmptyApp>("--help shows help - no commands")
                {
                    Args = "--help",
                    Help = @"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information"
                },
                new Scenario<EmptyApp>("-h shows help - no commands")
                {
                    Args = "-h",
                    Help = @"Usage: dotnet testhost.dll [options]

Options:

  -v | --version          
  Show version information

  -h | --help             
  Show help information"
                },
                new Scenario<EmptyApp>("--version shows version")
                {
                    Args = "--version",
                    Help = @"testhost.dll
15.9.0"
                },
                new Scenario<EmptyApp>("-v shows version")
                {
                    Args = "--version",
                    Help = @"testhost.dll
15.9.0"
                }
            };
    }
}