using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CommandDotNet.MicrosoftCommandLineUtils
{
    internal static class Directives
    {
        public class DirectivesResult
        {
            public int? ExitCode { get; }

            public DirectivesResult(int? exitCode = null)
            {
                ExitCode = exitCode;
            }
        }

        public static DirectivesResult ProcessDirectives(ref string[] args)
        {
            var firstArg = args.FirstOrDefault();
            if (firstArg == null)
            {
                return new DirectivesResult();
            }

            // adapted from https://github.com/dotnet/command-line-api directives

            if (firstArg.Equals("[debug]", StringComparison.InvariantCultureIgnoreCase))
            {
                args = args.Skip(1).ToArray();
                var process = Process.GetCurrentProcess();

                var processId = process.Id;

                Console.Out.WriteLine($"Attach your debugger to process {processId} ({process.ProcessName}).");

                while (!Debugger.IsAttached)
                {
                    Task.Delay(500);
                }
            }
            else if (firstArg.Equals("[parse]", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var arg in args.Skip(1))
                {
                    Console.Out.WriteLine(arg);
                }
                return new DirectivesResult(0);
            }


            return new DirectivesResult();
        }

    }
}