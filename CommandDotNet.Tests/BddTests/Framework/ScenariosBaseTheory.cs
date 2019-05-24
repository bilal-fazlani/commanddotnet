using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;

namespace CommandDotNet.Tests.BddTests.Framework
{
    public abstract class ScenariosBaseTheory
    {
        public virtual string DescriptionForTestName => null;
        public virtual AppSettings DefaultAppSettings => new AppSettings();
        public abstract Scenarios Scenarios { get; }

        public IEnumerable<IScenario> GetActive()
        {
            return Get(includeActive: true);
        }
        
        public IEnumerable<IScenario> GetSkipped()
        {
            return Get(includeSkipped: true);
        }
        
        public IEnumerable<IScenario> GetAll()
        {
            return Get(includeActive: true, includeSkipped: true);
        }

        private IEnumerable<IScenario> Get(bool includeActive = false, bool includeSkipped = false)
        {
            var context = new ScenarioContext
            {
                Host = this,
                Description = DescriptionForTestName
            };

            return Scenarios
                .Where(s => (includeActive && s.SkipReason == null) || (includeSkipped && s.SkipReason != null))
                .Select(s =>
                {
                    s.Context = context;
                    s.And.AppSettings = s.And.AppSettings ?? DefaultAppSettings;
                    return s;
                });
        }
    }
}