using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace CommandDotNet.DocExamples.GettingStarted
{
    [TestFixture]
    public class Getting_Started_800_CtrlC
    {
        // begin-snippet: getting-started-800-ctrlc
        public class Program
        {
            static int Main(string[] args) => AppRunner.Run(args);

            public static AppRunner AppRunner => 
                new AppRunner<Program>()
                    .UseCancellationHandlers();

            // could also use .UseDefaultMiddleware()

            public void Range(IConsole console, CancellationToken ct, int start, int count, int sleep = 0)
            {
                foreach (var i in Enumerable.Range(start, count).UntilCancelled(ct, sleep))
                {
                    console.WriteLine(i);
                }
            }

            public void Sum(IConsole console, CancellationToken ct, IEnumerable<int> values)
            {
                int total = 0;
                foreach (var value in values.ThrowIfCancelled(ct))
                {
                    console.WriteLine(total += value);
                }
            }
        }
        // end-snippet

        public static BashSnippet Range = new("getting-started-800-ctrlc_range",
            Program.AppRunner,
            "dotnet linq.dll", "Range 1 4", 0,
            @"1
2
3
4");

        public static BashSnippet Sum = new("getting-started-800-ctrlc_sum",
            Program.AppRunner,
            "dotnet linq.dll", "Sum 1 2 3 4", 0,
            @"1
3
6
10");

        [Test] public void Obligatory_test_since_snippets_cover_all_cases() => Assert.True(true);
    }
}