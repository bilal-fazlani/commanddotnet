using CommandDotNet.Tests.SmokeTests.Apps;

namespace CommandDotNet.Tests.SmokeTests.TestScenarios
{
    public class SingleMethodAppDetailedHelpScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Scenario<SingleMethodApp>("default help shows command and example")
                {
                    Args = "-h",
                    Help = @"Usage: dotnet testhost.dll [options] [command]

Options:

  -v | --version
  Show version information

  -h | --help
  Show help information


Commands:

  Add

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                },
                new Scenario<SingleMethodApp>("help for command shows arguments and options")
                {
                    Args = "Add -h",
                    Help = @"Usage: dotnet testhost.dll Add [arguments] [options]

Arguments:

  x    <NUMBER>

  y    <NUMBER>


Options:

  -h | --help
  Show help information"
                }
            };
    }
}