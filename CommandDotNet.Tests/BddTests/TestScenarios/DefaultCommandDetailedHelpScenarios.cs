using CommandDotNet.Tests.BddTests.Apps;
using CommandDotNet.Tests.BddTests.Framework;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class DefaultCommandDetailedHelpScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<DefaultCommandNoArgsApp>("default method help is same as NoCommandApp")
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
                new Given<DefaultCommandWithArgsApp>("default method help shows arguments")
                {
                    SkipReason = "Known Issue #24",
                    WhenArgs = "-h",
                    Then =
                    {
                        Help = @"Usage: dotnet testhost.dll [options]

Arguments:

  text    <TEXT>
  some text

Options:

  -v | --version
  Show version information

  -h | --help
  Show help information"
                    }
                }
            };
    }
}