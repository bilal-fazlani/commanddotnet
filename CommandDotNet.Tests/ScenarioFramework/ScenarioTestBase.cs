using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public abstract class ScenarioTestBase<TSuper>
    {
        private readonly ScenarioVerifier _scenarioVerifier;

        private static readonly ScenarioTestData TestData = GetAllScenarios();

        public static IEnumerable<object[]> ActiveScenarios => TestData.ActiveTheories;

        public static IEnumerable<object[]> SkippedScenarios => TestData.SkippedTheories;

        public static IEnumerable<object[]> NotSupportedScenarios => TestData.NotSupportedTheories;

        private static ScenarioTestData GetAllScenarios()
        {
            var scenarioPropName = "Scenarios";

            var scenariosProp = typeof(TSuper).GetProperty(scenarioPropName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (scenariosProp == null)
            {
                throw new Exception($"test class must define static property `{scenarioPropName}`");
            }

            var scenariosRaw = scenariosProp.GetValue(null, null);
            if (scenariosRaw == null)
            {
                throw new Exception($"test class static property `{scenarioPropName}` cannot be null");
            }

            var scenarios = scenariosRaw as IEnumerable<IScenario>;
            if (scenariosProp == null)
            {
                throw new Exception($"test class static property `{scenarioPropName}` ({scenariosRaw?.GetType()}) must implement {typeof(IEnumerable<IScenario>)}");
            }
            
            return new ScenarioTestData(scenarios, TestAppSettings.TestDefault);
        }

        public ScenarioTestBase(ITestOutputHelper output)
        {
            _scenarioVerifier = new ScenarioVerifier(output);
        }

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
        public void Active(IScenario scenario)
        {
            _scenarioVerifier.VerifyScenario(scenario);
        }
    }
}