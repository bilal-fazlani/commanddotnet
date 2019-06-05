using System.Collections.Generic;
using CommandDotNet.Models;

namespace CommandDotNet.Tests.BddTests.Framework
{
    public abstract class ScenariosBaseTheory
    {
        public virtual string DescriptionForTestName => null;
        public virtual AppSettings DefaultAppSettings => new AppSettings();
        public abstract Scenarios Scenarios { get; }

        private ScenarioTestData _scenarioTestData;

        public ScenarioTestData ScenarioTestData => 
            _scenarioTestData ?? (_scenarioTestData = new ScenarioTestData(
                Scenarios,
                DefaultAppSettings, new ScenarioContext {Host = this, Description = DescriptionForTestName}));

        public IEnumerable<IScenario> GetActive()
        {
            return ScenarioTestData.ActiveScenarios;
        }
        
        public IEnumerable<IScenario> GetSkipped()
        {
            return ScenarioTestData.SkippedScenarios;
        }

        public IEnumerable<IScenario> GetNotSupported()
        {
            return ScenarioTestData.NotSupportedScenarios;
        }
    }
}