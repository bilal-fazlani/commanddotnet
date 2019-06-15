using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public abstract class ScenarioTestBase<TSuper> : TestBase
    {
        public ScenarioTestBase(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [MemberData(nameof(GetScenarios))]
        public void Active(IScenario scenario)
        {
            Verify(scenario);
        }

        public static IEnumerable<object[]> GetScenarios()
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

            return new ScenarioTestData(scenarios, TestAppSettings.TestDefault).ActiveTheories;
        }
    }
}