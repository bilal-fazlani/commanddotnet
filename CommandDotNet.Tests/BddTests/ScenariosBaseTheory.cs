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
            return this.Scenarios.Select(s => new Object[] { s }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}