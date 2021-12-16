using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
    [TestFixture]
    public class Getting_Started_700_Pipes
    {
        // begin-snippet: getting-started-700-pipes
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);

            public static AppRunner AppRunner => new AppRunner<Program>();

            public void Range(IConsole console, int start, int count, int sleep = 0)
            {
                foreach (var i in Enumerable.Range(start, count))
                {
                    console.WriteLine(i);
                    if (sleep > 0)
                    {
                        Thread.Sleep(sleep);
                    }
                }
            }

            public void Sum(IConsole console, IEnumerable<int> values)
            {
                int total = 0;
                foreach (var value in values)
                {
                    console.WriteLine(total += value);
                }
            }
        }
        // end-snippet

        public static BashSnippet Range = new("getting-started-700-pipes_range",
            Program.AppRunner,
            "dotnet linq.dll", "Range 1 4", 0,
            @"1
2
3
4");

        public static BashSnippet Sum = new("getting-started-700-pipes_sum",
            Program.AppRunner,
            "dotnet linq.dll", "Sum 1 2 3 4", 0,
            @"1
3
6
10");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}