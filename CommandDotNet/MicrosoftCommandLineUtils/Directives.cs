using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Models;
using CommandDotNet.Parsing;

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

        // this is obviously not the ideal design.  major code smell.
        // but... it meets our needs simply until we have settled
        // on a better design for implementing the control flow
        // i.e. middleware pipeline
        internal static bool InTestHarness { private get; set; }

        public static DirectivesResult ProcessDirectives(AppSettings appSettings, ParserContext parserContext, ref string[] args)
        {
            bool IsDirective(string directiveName, ref string[] arguments)
            {
                return arguments.FirstOrDefault()?.Equals($"[{directiveName}]", StringComparison.InvariantCultureIgnoreCase) ?? false;
            }

            // adapted from https://github.com/dotnet/command-line-api directives

            if (IsDirective("debug", ref args))
            {
                args = args.Skip(1).ToArray();
                var process = Process.GetCurrentProcess();

                var processId = process.Id;

                appSettings.Out.WriteLine($"Attach your debugger to process {processId} ({process.ProcessName}).");

                while (!InTestHarness && !Debugger.IsAttached)
                {
                    Task.Delay(500);
                }
            }
            
            if (IsDirective("parse", ref args))
            {
                args = args.Skip(1).ToArray();
                parserContext.ParseDirectiveEnabled = true;
            }

            return new DirectivesResult();
        }

    }
}