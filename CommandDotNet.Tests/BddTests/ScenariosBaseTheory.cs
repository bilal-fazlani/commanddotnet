using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;

namespace CommandDotNet.Tests.BddTests
{
    public abstract class ScenariosBaseTheory : IEnumerable<object[]>
    {
        public virtual string DescriptionForTestName => null;
        public virtual AppSettings DefaultAppSettings => new AppSettings();
        public abstract Scenarios Scenarios { get; }

        public IEnumerator<object[]> GetEnumerator()
        {
            var context = new ScenarioContext
            {
                Host = this,
                Description = DescriptionForTestName
            };

            // exclude skipped tests until xunit allows us to
            // skip them in a way so they appear as skipped
            // in the test runner
            return Scenarios
                .Where(s => s.SkipReason == null)
                .Select(s =>
                {
                    s.Context = context;
                    s.And.AppSettings = s.And.AppSettings ?? DefaultAppSettings;
                    return new Object[] {s};
                })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}