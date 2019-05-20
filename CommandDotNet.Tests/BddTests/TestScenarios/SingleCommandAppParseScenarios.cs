using CommandDotNet.Tests.BddTests.Apps;

namespace CommandDotNet.Tests.BddTests.TestScenarios
{
    public class SingleCommandAppParseScenarios : ScenariosBaseTheory
    {
        public override Scenarios Scenarios =>
            new Scenarios
            {
                new Given<SingleCommandApp>("method is called with expected values")
                {
                    WhenArgs = "Add 2 3",
                    Then = {Outputs = {new SingleCommandApp.AddResults {X = 2, Y = 3}}}
                }
            };
    }
}