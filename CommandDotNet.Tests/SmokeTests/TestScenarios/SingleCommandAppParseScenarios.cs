using CommandDotNet.Tests.SmokeTests.Apps;

namespace CommandDotNet.Tests.SmokeTests.TestScenarios
{
    public class SingleCommandAppParseScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Scenario<SingleCommandApp>("method is called with expected values")
                {
                    Args = "Add 2 3",
                    Outputs = { new SingleCommandApp.AddResults { X = 2, Y = 3 } }
                }
            };
    }
}