using CommandDotNet.Tests.BddTests.Apps;
using CommandDotNet.Tests.BddTests.Framework;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class SubCommandScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<SubCommandApp>("help lists sub-commands")
                {
                    WhenArgs = null,
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll [options] [command]

Options:

  -v | --version
  Show version information

  -h | --help
  Show help information


Commands:

  Math

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                },
                new Given<SubCommandApp>("help for sub-command shows sub-command options and sub-commands")
                {
                    WhenArgs = "Math",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll Math [options] [command]

Options:

  -h | --help
  Show help information


Commands:

  Add

Use ""dotnet testhost.dll Math [command] --help"" for more information about a command."
                    }
                },
                new Given<SubCommandApp>("help for nested sub-command shows nested sub-command arguments and options")
                {
                    WhenArgs = "Math Add -h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll Math Add [arguments] [options]

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