using CommandDotNet.Tests.ScenarioFramework;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
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