using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CommandDotNet.Example.DocExamples.GettingStarted.Eg5_Pipes
{
    // begin-snippet: getting_started_pipes
    public class Program
    {
        static int Main(string[] args) =>
            new AppRunner<Program>().Run(args);

        public void Range(int start, int count, int sleep = 0)
        {
            foreach (var i in Enumerable.Range(start, count))
            {
                Console.WriteLine(i);
                if (sleep > 0)
                {
                    Thread.Sleep(sleep);
                }
            }
        }

        public void Sum(IEnumerable<int> values)
        {
            int total = 0;
            foreach (var value in values)
            {
                Console.WriteLine(total+=value);
            }
        }
    }
    // end-snippet
}