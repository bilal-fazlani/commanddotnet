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
    }
}