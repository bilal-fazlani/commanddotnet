using CommandDotNet.Tests.SmokeTests.TestScenarios;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.SmokeTests
{
    public class ScenarioTests
    {
        [Theory]
        [ClassData(typeof(DefaultHelpAndVersionDetailedHelpScenarios))]
        [ClassData(typeof(SingleMethodAppDetailedHelpScenarios))]
        public void Help(IScenario scenario)
        {
            var results = scenario.AppType.RunAppInMem(scenario.Args);
            results.ExitCode.Should().Be(scenario.ExitCode.GetValueOrDefault());
            results.HelpShouldBe(scenario.Help);
        }

        [Theory]
        [ClassData(typeof(SingleCommandAppParseScenarios))]
        public void Parse(IScenario scenario)
        {
            var results = scenario.AppType.RunAppInMem(scenario.Args);
            results.ExitCode.Should().Be(scenario.ExitCode.GetValueOrDefault());
            foreach (var expectedOutput in scenario.Outputs)
            {
                var actualOutput = results.TestOutputs.Get(expectedOutput.GetType());
                actualOutput.Should().NotBeNull(because:$"{expectedOutput.GetType().Name} should have been output in test run");
                actualOutput.Should().BeEquivalentTo(expectedOutput);
            }
        }
    }
}