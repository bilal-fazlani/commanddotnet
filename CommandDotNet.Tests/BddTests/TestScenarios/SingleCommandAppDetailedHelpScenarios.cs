using CommandDotNet.Tests.BddTests.Apps;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class SingleCommandAppDetailedHelpScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<SingleCommandApp>("default help shows command and example")
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll [options] [command]

Options:

  -v | --version
  Show version information

  -h | --help
  Show help information


Commands:

  Add

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<SingleCommandApp>("help for command shows arguments and options")
                {
                    WhenArgs = "Add -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll Add [arguments] [options]

Arguments:

  x    <NUMBER>
  the first operand

  y    <NUMBER>
  the second operand


Options:

  -h | --help
  Show help information

  -o | --operator    <TEXT>    [+]
  the operation to apply"
                    }
                }
            };
    }
}