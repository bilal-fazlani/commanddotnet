using System;
using System.Threading;

namespace CommandDotNet.Example.Commands
{
    [Command(
        Description = "Example of using cancellation tokens. " +
                      "Use this to test changes to CancellationMiddleware across OS's",
        Usage = "`dotnet CommandDotNet.Examples.dll cancel-me` the app will run until Ctrl+C is entered.\n" +
                "`dotnet CommandDotNet.Examples.dll cancel-me -x 10` to trigger Environment.Exit from another thread to ensure the app will exit.\n" +
                "`dotnet CommandDotNet.Examples.dll cancel-me -c 10` to raise an unhandled exception to ensure the app will exit."
        )]
    public class CancelMe
    {
        [DefaultCommand]
        public void Default(
            CancellationToken cancellationToken, 
            IConsole console,
            IEnvironment environment,
            [Option('x')] int exitAfterNRounds = -1,
            [Option('c')] int crashAfterNRounds = -1)
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
                    new Thread(() => environment.Exit(0)).Start();
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