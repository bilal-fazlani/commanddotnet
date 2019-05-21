using CommandDotNet.Models;
using CommandDotNet.Tests.BddTests.Apps;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class ArgumentSeparatorHelpScenarios : ScenariosBaseTheory
    {
        private readonly AppSettings _argSeparatorEnabled = new AppSettings { AllowArgumentSeparator = true };

        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<SingleCommandApp>("help example includes argument separator")
                {
                    And = {AppSettings = _argSeparatorEnabled},
                    WhenArgs = "Add -h",
                    Then =
                    {
                        HelpContainsTexts = {@"Usage: dotnet testhost.dll Add [arguments] [options] [[--] <arg>...]"}
                    }
                }
            };
    }
}