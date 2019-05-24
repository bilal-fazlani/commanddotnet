using CommandDotNet.Tests.BddTests.Apps;
using CommandDotNet.Tests.BddTests.Framework;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class DefaultCommandParseScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<DefaultCommandNoArgsApp>("default method - executes")
                {
                    WhenArgs = "",
                    Then =
                    {
                        Outputs = { DefaultCommandNoArgsApp.DefaultMethodExecuted }
                    }
                },
                new Given<DefaultCommandWithArgsApp>("default method - executes with args")
                {
                    SkipReason = "Known Issue #24",
                    WhenArgs = "abcde",
                    Then =
                    {
                        Outputs = { "abcde" }
                    }
                }
            };
    }
}