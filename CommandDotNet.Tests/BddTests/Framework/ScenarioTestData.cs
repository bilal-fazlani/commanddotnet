using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;

namespace CommandDotNet.Tests.BddTests.Framework
{
    public class ScenarioTestData
    {
        public IEnumerable<IScenario> ActiveScenarios { get; }
        public IEnumerable<IScenario> SkippedScenarios { get; }
        public IEnumerable<IScenario> NotSupportedScenarios { get; }

        public IEnumerable<object[]> ActiveTheories { get; }
        public IEnumerable<object[]> SkippedTheories { get; }
        public IEnumerable<object[]> NotSupportedTheories { get; }

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

            ActiveScenarios = all.Where(s => s.SkipReason == null && s.NotSupportedReason == null);
            SkippedScenarios = all.Where(s => s.SkipReason != null);
            NotSupportedScenarios = all.Where(s => s.NotSupportedReason != null);

            ActiveTheories = ActiveScenarios.ToObjectArrays();
            SkippedTheories = SkippedScenarios.ToObjectArrays();
            NotSupportedTheories = NotSupportedScenarios.ToObjectArrays();
        }
    }
}