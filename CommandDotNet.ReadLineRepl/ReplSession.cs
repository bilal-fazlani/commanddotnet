using System;
using System.Linq;
using CommandDotNet.Tokens;

namespace CommandDotNet.ReadLineRepl
{
    public class ReplSession
    {
        private readonly AppRunner _appRunner;
        private readonly ReplConfig _replConfig;
        private readonly CommandContext _context;

        public ReplSession(AppRunner appRunner, ReplConfig replConfig, CommandContext context)
        {
            _appRunner = appRunner;
            _replConfig = replConfig;
            _context = context;
        }

        public void Start()
        {
            var console = _context.Console;
            var cancellationToken = _context.CancellationToken;

            bool pressedCtrlC = false;
            Console.CancelKeyPress += (sender, args) =>
            {
                pressedCtrlC = true;
            };

            var sessionInitMessage = _replConfig.GetSessionInitMessage(_context);
            var sessionHelpMessage = _replConfig.GetSessionHelpMessage(_context);

            console.WriteLine(sessionInitMessage);

            bool pendingNewLine = false;
            void Write(string? value = null)
            {
                console!.Write(value);
                pendingNewLine = true;
            }

            void WriteLine(string? value = null)
            {
                console!.WriteLine(value);
                pendingNewLine = false;
            }

            void EnsureNewLine()
            {
                if (pendingNewLine)
                {
                    WriteLine();
                }
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                EnsureNewLine();
                Write(">>>");
                var input = ReadLine.Read();
                if (input is null || pressedCtrlC)
                {
                    pressedCtrlC = false;
                    WriteLine();
                    return;
                }

                var args = new CommandLineStringSplitter().Split(input).ToArray();
                if (args.Length == 0)
                {
                    WriteLine();
                    continue;
                }
                if (args.Length == 1)
                {
                    var singleArg = args[0];
                    switch (singleArg)
                    {
                        case "exit":
                        case "quit":
                            return;
                        case "help":
                            console.WriteLine(sessionHelpMessage);
                            continue;
                    }
                    if (singleArg == Environment.NewLine)
                    {
                        WriteLine();
                        continue;
                    }
                }
                EnsureNewLine();
                _appRunner.Run(args);
            }
            EnsureNewLine();
        }
    }
}