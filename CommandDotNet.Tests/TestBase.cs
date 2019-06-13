using System;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class TestBase
    {
        protected readonly ITestOutputHelper TestOutputHelper;

        private readonly ScenarioVerifier _scenarioVerifier;

        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
            _scenarioVerifier = new ScenarioVerifier(testOutputHelper);

            // WARNING: this doesn't play well with parallelized tests
            var testConsoleWriter = new TestConsoleWriter(testOutputHelper);
            Console.SetOut(testConsoleWriter);
            Console.SetError(testConsoleWriter);
        }

        protected void Verify(IScenario scenario)
        {
            _scenarioVerifier.VerifyScenario(scenario);
        }
    }
}