using System;
using System.Linq;
using CommandDotNet.Tokens;

namespace CommandDotNet.Repl
{
    public class ReplSession
    {
        /* Test:
         * SessionContext contains parent
         * - options provided during session creation are available
         *
         * - CtrlC
         *   - if command running in session
         *     - cancels command
         *     - else closes session
         * - prints session init message
         * - exit & quit closes session
         * - help shows session help
         * - -h & --help show command help
         * - can run commands and return to session
         *
         * - to support nested sessions
         *   - ReplSession via parameter resolver
         *   - ReplConfig can be changed and doesn't change the parent. i.e. different prompt text
         *
         * - ReplSession.Start works when called within an async method
         */

        private readonly AppRunner _appRunner;
        private ReplConfig _replConfig;

        public ReplConfig ReplConfig
        {
            get => _replConfig;
            set => _replConfig = value ?? throw new ArgumentNullException(nameof(value));
        }

        public CommandContext SessionContext { get; }

        public ReplSession(AppRunner appRunner, ReplConfig replConfig, CommandContext sessionContext)
        {
            _appRunner = appRunner ?? throw new ArgumentNullException(nameof(appRunner));
            _replConfig = replConfig ?? throw new ArgumentNullException(nameof(replConfig));
            SessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
        }

        public void Start()
        {
            var console = SessionContext.Console;
            var cancellationToken = SessionContext.CancellationToken;

            bool pressedCtrlC = false;
            Console.CancelKeyPress += (sender, args) =>
            {
                pressedCtrlC = true;
            };

            var sessionInitMessage = ReplConfig.SessionInitMessageCallback(SessionContext);
            var sessionHelpMessage = ReplConfig.SessionHelpMessageCallback(SessionContext);

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
                Write(ReplConfig.PromptTextCallback(SessionContext));
                var input = ReplConfig.ReadLine!(SessionContext);
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