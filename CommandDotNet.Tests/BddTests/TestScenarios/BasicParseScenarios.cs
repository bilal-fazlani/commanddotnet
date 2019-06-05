using CommandDotNet.Tests.BddTests.Apps;
using CommandDotNet.Tests.BddTests.Framework;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class BasicParseScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<SingleCommandApp>("method is called with expected values")
                {
                    WhenArgs = "Add -o * 2 3",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3, Op = "*"}}}
                },
                new Given<SingleCommandApp>("option not required")
                {
                    WhenArgs = "Add 2 3",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3, Op = "+"}}}
                },
                new Given<SingleCommandApp>("option can be specified after positional arg")
                {
                    WhenArgs = "Add 2 3 -o *",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3, Op = "*"}}}
                },
                new Given<SingleCommandApp>("option can be colon separated: --option:value")
                {
                    WhenArgs = "Add 2 3 -o:*",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3, Op = "*"}}}
                },
                new Given<SingleCommandApp>("option can be specified after positional arg")
                {
                    WhenArgs = "Add 2 3 -o=*",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3, Op = "*"}}}
                },
                new Given<SingleCommandApp>("error when extra value provided for option")
                {
                    WhenArgs = "Add 2 3 -o * %",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = {"Unrecognized command or argument '%'"}
                    }
                },
                new Given<SingleCommandApp>("extra arguments not allowed")
                {
                    WhenArgs = "Add 2 3 4",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = {"Unrecognized command or argument '4'"}
                    }
                },
                new Given<SingleCommandApp>("positional arguments are required")
                {
                    SkipReason = "Not implemented yet",
                    WhenArgs = "Add 2",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = {"missing positional argument 'Y'"}
                    }
                }
            };
    }
}