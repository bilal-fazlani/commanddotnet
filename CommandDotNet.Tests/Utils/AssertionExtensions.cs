using FluentAssertions;
using FluentAssertions.Collections;

namespace CommandDotNet.Tests.Utils
{
    public static class AssertionExtensions
    {
        /// <summary>Should be same order</summary>
        public static AndConstraint<StringCollectionAssertions> BeEquivalentSequenceTo(
            this StringCollectionAssertions assertions,
            params string[] expectedValues)
        {
            return assertions.BeEquivalentTo(expectedValues, c => c.WithStrictOrderingFor(s => s));
        }
    }
}