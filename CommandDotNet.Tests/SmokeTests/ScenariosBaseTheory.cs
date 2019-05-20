using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Tests.SmokeTests
{
    public abstract class ScenariosBaseTheory : IEnumerable<object[]>
    {
        public abstract Scenarios Scenarios { get; }

        public IEnumerator<object[]> GetEnumerator()
        {
            return Scenarios.Select(s => new Object[] { s }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}