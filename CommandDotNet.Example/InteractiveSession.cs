using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Tokens;

namespace CommandDotNet.Example;

internal class InteractiveSession(AppRunner appRunner, string appName, CommandContext context)
{
    
    [SuppressMessage("ReSharper", "CognitiveComplexity", Justification = "The code is more complicated when split")]
    public void Start()
    {
        var console = context.Console;
        var cancellationToken = context.CancellationToken;

        bool pressedCtrlC = false;
        Console.CancelKeyPress += (_, _) =>
        {
            pressedCtrlC = true;
        };

        PrintSessionInit();

        bool pendingNewLine = false;

        while (!cancellationToken.IsCancellationRequested)
        {
            EnsureNewLine();
            Write(">>>");
            var input = console.In.ReadLine();
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
                        PrintSessionHelp();
                        continue;
                }
                if (singleArg == Environment.NewLine)
                {
                    WriteLine();
                    continue;
                }
            }
            EnsureNewLine();
            appRunner.Run(args);
        }
        EnsureNewLine();
        return;

        void Write(string? value = null)
        {
            console.Write(value);
            pendingNewLine = true;
        }

        void WriteLine(string? value = null)
        {
            console.WriteLine(value);
            pendingNewLine = false;
        }

        void EnsureNewLine()
        {
            if (pendingNewLine)
            {
                WriteLine();
            }
        }
    }

    private void PrintSessionInit()
    {
        var appInfo = AppInfo.Instance;
        var console = context.Console;
        console.WriteLine($"{appName} {appInfo.Version}");
        console.WriteLine("Type 'help' to see interactive options");
        console.WriteLine("Type '-h' or '--help' to options for commands");
        console.WriteLine("Type 'exit', 'quit' or 'Ctrl+C' to exit.");
    }

    private void PrintSessionHelp()
    {
        var console = context.Console;
        console.WriteLine("Type '-h' or '--help' to options for commands");
        console.WriteLine("Type 'exit', 'quit' or 'Ctrl+C' to exit.");
    }
}