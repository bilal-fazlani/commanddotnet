using System;
using System.Collections.Generic;
using System.Threading;
using CommandDotNet.Rendering;

namespace CommandDotNet.Example
{
    [Command(Name = "pipes", 
        Description = "example of accepting piped input",
        Usage = "`... pipes Echo --times 10 hello | ... pipes Echo` to see piped input printed again\n" +
                "`... pipes Echo --times 10 --sleep 10 hello | ... pipes Echo` to see that piped input can be streamed as available")]
    public class PipesApp
    {
        [Command(
            Usage = "`... pipes Echo --times 10 hello | ... pipes Echo` to see piped input printed again\n"
                    + "`... pipes Echo --times 10 --sleep 10 hello | ... pipes Echo` to see that piped input can be streamed")]
        public void Echo(
            IConsole console,
            CancellationToken cancellationToken,
            [Operand] IEnumerable<string> inputs, 
            [Option] int times = 1, 
            [Option(Description = "sleep N seconds between each echo")] int sleep = 0)
        {
            if (inputs == null)
            {
                return;
            }

            foreach (var input in inputs)
            {
                for (int i = 0; i < times; i++)
                {
                    console.Out.WriteLine(input);
                    if (sleep > 0)
                    {
                        cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(sleep));
                    }
                }
            }
        }
    }
}