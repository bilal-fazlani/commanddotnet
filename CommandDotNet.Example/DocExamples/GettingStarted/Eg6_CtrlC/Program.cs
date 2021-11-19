using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CommandDotNet.Example.DocExamples.GettingStarted.Eg6_CtrlC
{
    // begin-snippet: getting_started_ctrlc
    public class Program
    {
        static int Main(string[] args) =>
            new AppRunner<Program>()
                .UseCancellationHandlers()
                .Run(args);

        public void Range(CancellationToken ct, int start, int count, int sleep = 0)
        {
            foreach (var i in Enumerable.Range(start, count).UntilCancelled(ct, sleep))
            {
                Console.WriteLine(i);
            }
        }

        public void Sum(CancellationToken ct, IEnumerable<int> values)
        {
            int total = 0;
            foreach (var value in values.ThrowIfCancelled(ct))
            {
                Console.WriteLine(total+=value);
            }
        }
    }
    // end-snippet
}