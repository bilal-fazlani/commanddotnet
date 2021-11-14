using System;
using System.IO;
using System.Text;
using System.Threading;
using CommandDotNet.Rendering;
using Spectre.Console;

namespace CommandDotNet.Spectre
{
    public class AnsiConsoleForwardingConsole : SystemConsole
    {
        private readonly IAnsiConsole _ansiConsole;

        public AnsiConsoleForwardingConsole(IAnsiConsole ansiConsole)
        {
            _ansiConsole = ansiConsole;
            Out = new ForwardingTextWriter(ansiConsole.Write!);
        }

        public override Encoding OutputEncoding
        {
            get => _ansiConsole.Profile.Encoding;
            set => _ansiConsole.Profile.Encoding = value;
        }

        public override TextWriter Out { get; }

        public override int WindowWidth
        {
            get => _ansiConsole.Profile.Width;
            set => _ansiConsole.Profile.Width = value;
        }

        public override int WindowHeight
        {
            get => _ansiConsole.Profile.Height;
            set => _ansiConsole.Profile.Height = value;
        }

        public override ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            var consoleKeyInfo = _ansiConsole.Input.ReadKeyAsync(intercept, CancellationToken.None).Result;
            return (ConsoleKeyInfo)consoleKeyInfo;
        }
    }
}