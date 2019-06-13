using Xunit.Abstractions;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public class TestBase
    {
        private readonly ScenarioVerifier _scenarioVerifier;

        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            _scenarioVerifier = new ScenarioVerifier(testOutputHelper);
        }

        protected void Verify(IScenario scenario)
        {
            _scenarioVerifier.VerifyScenario(scenario);
        }
    }
}