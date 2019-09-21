using System;
using System.Threading;
using CommandDotNet.Rendering;

namespace CommandDotNet.Example
{
    [Command(Name = "cancel-me", Description = "example of using cancellation tokens. Also used to test the CancellationMiddleware")]
    public class CancelMeApp
    {
        [DefaultMethod]
        public void Default(
            CancellationToken cancellationToken, 
            IConsole console,
            [Option(ShortName = "x", LongName = "exitAfterNRounds")] int exitAfterNRounds = -1,
            [Option(ShortName = "c", LongName = "crashAfterNRounds")] int crashAfterNRounds = -1
        )
        {
            int counter = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                console.Out.WriteLine($"press Ctrl+C to exit ({counter})");
                Thread.Sleep(1000);
                counter++;

                if (counter == exitAfterNRounds)
                {
                    console.Out.WriteLine("exiting app in new thread");
                    new Thread(() => Environment.Exit(0)).Start();
                }
                if(counter == crashAfterNRounds)
                {
                    console.Out.WriteLine("crashing app in new thread");
                    new Thread(() => throw new Exception("Unrecoverable exception")).Start();
                }
            }

            console.Out.WriteLine($"cancellationToken.IsCancellationRequested={cancellationToken.IsCancellationRequested}");
        }
    }
}