using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public class ScenarioTestData
    {
        public IEnumerable<IScenario> ActiveScenarios { get; }

        public IEnumerable<object[]> ActiveTheories { get; }

        public ScenarioTestData(
            IEnumerable<IScenario> scenarios, 
            AppSettings defaultAppSettings,
            IScenarioContext context = null)
        {
            var all = scenarios.Select(s =>
            {
                s.Context = context;
                s.And.AppSettings = s.And.AppSettings ?? defaultAppSettings;
                return s;
            }).ToList();

            ActiveScenarios = all;
            ActiveTheories = ActiveScenarios.ToObjectArrays();
        }
    }
}