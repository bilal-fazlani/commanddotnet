using CommandDotNet.Tests.BddTests.Apps;
using CommandDotNet.Tests.BddTests.Framework;

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
                        Help = @"Usage: Math [options] [command]

Options:

  -v | --version
  Show version information

  -h | --help
  Show help information


Commands:

  Add

Use ""Math [command] --help"" for more information about a command."
                    }
                },
                new Given<SingleCommandApp>("help for command shows arguments and options")
                {
                    WhenArgs = "Add -h",
                    Then =
                    {
                        Help = @"Usage: Math Add [arguments] [options]

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