using System;
using System.Collections.Generic;
using System.Threading;

namespace CommandDotNet.Example.Commands
{
    [Command(
        Description = "demonstrates accepting piped input",
        Usage = "`pipes echo --times 10 hello | pipes echo` to see piped input printed again\n" +
                "`pipes echo --times 10 --sleep 10 hello | pipes echo` to see that piped input can be streamed as available")]
    public class Pipes
    {
        [Command(
            Usage = "`pipes echo --times 10 hello | pipes echo` to see piped input printed again\n"
                    + "`pipes echo --times 10 --sleep 10 hello | pipes echo` to see that piped input can be streamed")]
        public void Echo(
            IConsole console,
            CancellationToken cancellationToken,
            [Operand] IEnumerable<string>? inputs,
            [Option('j', Description = "Use $* to direct piped input to this option")] IEnumerable<string>? hijack,
            [Option] int times = 1, 
            [Option(Description = "sleep N seconds between each echo")] int sleep = 0)
        {
            if (hijack is not null)
            {
                console.WriteLine("piped input directed to the hijack option");
                foreach (var s in hijack)
                {
                    console.WriteLine(s);
                }
            }
            else if (inputs is not null)
            {
                foreach (var input in inputs)
                {
                    for (int i = 0; i < times; i++)
                    {
                        console.WriteLine(input);
                        if (sleep > 0)
                        {
                            cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(sleep));
                        }
                    }
                }
            }
        }
    }
}