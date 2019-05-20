using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Tests.BddTests
{
    public abstract class ScenariosBaseTheory : IEnumerable<object[]>
    {
        public abstract Scenarios Scenarios { get; }

        public IEnumerator<object[]> GetEnumerator()
        {
            // exclude skipped tests until xunit allows us to
            // skip them in a way so they appear as skipped
            // in the test runner
            return Scenarios
                .Where(s => s.SkipReason == null)
                .Select(s => new Object[] { s })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}