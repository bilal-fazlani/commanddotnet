using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Tests.BddTests.Framework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.BddTests
{
    public class ScenarioTests
    {
        private static List<ScenariosBaseTheory> AllScenarios = typeof(ScenarioTests).Assembly.GetTypes()
            .Where(t => typeof(ScenariosBaseTheory).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<ScenariosBaseTheory>()
            .ToList();

        private readonly ScenarioVerifier _scenarioVerifier;

        public ScenarioTests(ITestOutputHelper output)
        {
            _scenarioVerifier = new ScenarioVerifier(output);
        }

        public static IEnumerable<object[]> ActiveScenarios =>
            AllScenarios
                .SelectMany(s => s.GetActive())
                .ToObjectArrays();

        public static IEnumerable<object[]> SkippedScenarios =>
            AllScenarios
                .SelectMany(s => s.GetSkipped())
                .ToObjectArrays();

        public static IEnumerable<object[]> NotSupportedScenarios =>
            AllScenarios
                .SelectMany(s => s.GetNotSupported())
                .ToObjectArrays();

        [Theory]
        [MemberData(nameof(SkippedScenarios),
            DisableDiscoveryEnumeration = true, 
            Skip = "skipped scenarios")]
        public void Skipped(IScenario scenario)
        {
        }

        [Theory]
        [MemberData(nameof(NotSupportedScenarios),
            DisableDiscoveryEnumeration = true, 
            Skip = "not-supported scenarios (features to consider)")]
        public void NotSupported(IScenario scenario)
        {
        }

        [Theory]
        [MemberData(nameof(ActiveScenarios))]
        public void Active(IScenario s)
        {
            // short parameter name to reduce redundant appearance of "scenario" in test name.
            _scenarioVerifier.VerifyScenario(s);
        }
    }
}