using CommandDotNet.Tests.BddTests.TestScenarios;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.BddTests
{
    public class ScenarioTests
    {
        [Theory]
        [ClassData(typeof(DefaultHelpAndVersionDetailedHelpScenarios))]
        [ClassData(typeof(SingleCommandAppDetailedHelpScenarios))]
        public void Help(IScenario scenario)
        {
            var results = scenario.GivenAppType.RunAppInMem(scenario.WhenArgs);
            results.ExitCode.Should().Be(scenario.Then.ExitCode.GetValueOrDefault());
            results.HelpShouldBe(scenario.Then.Help);
        }

        [Theory]
        [ClassData(typeof(SingleCommandAppParseScenarios))]
        public void Parse(IScenario scenario)
        {
            var results = scenario.GivenAppType.RunAppInMem(scenario.WhenArgs);
            results.ExitCode.Should().Be(scenario.Then.ExitCode.GetValueOrDefault());
            foreach (var expectedOutput in scenario.Then.Outputs)
            {
                var actualOutput = results.TestOutputs.Get(expectedOutput.GetType());
                actualOutput.Should().NotBeNull(because:$"{expectedOutput.GetType().Name} should have been output in test run");
                actualOutput.Should().BeEquivalentTo(expectedOutput);
            }
        }
    }
}