using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Tests.ScenarioFramework
{
    public static class XunitTestExtensions
    {
        // TODO: read from config?
        /// <summary>
        /// XUnit runners don't always play well with empty theory data sets.
        /// When true, we get an accurate count of skipped tests but sometimes test runs are aborted for these tests.
        /// When false, test runs always pass but skipped tests with no theories will count as 1 test, making hard to identify
        /// actual skipped tests
        /// </summary>
        private const bool DisableEmptyScenario = false;

        public static IEnumerable<object[]> ToObjectArrays<T>(this IEnumerable<T> items)
        {
            if (DisableEmptyScenario)
            {
                return items.Select(item => new object[] {item});
            }

            var list = items.ToList();
            return list.Count == 0
                ? new List<object[]> { new object[] {new EmptyScenario() } }
                : list.Select(item => new object[] { item });
        }
    }
}