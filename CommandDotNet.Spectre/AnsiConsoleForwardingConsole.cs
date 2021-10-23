﻿using System;
using CommandDotNet.Rendering;
using Spectre.Console;

namespace CommandDotNet.Spectre
{
    public class AnsiConsoleForwardingConsole : IConsole
    {
        private readonly IAnsiConsole _ansiConsole;

        public AnsiConsoleForwardingConsole(IAnsiConsole ansiConsole)
        {
            _ansiConsole = ansiConsole;
            Out = StandardStreamWriter.Create(ansiConsole.Write!);
            Error = StandardStreamWriter.Create(ansiConsole.Write!);
            In = StandardStreamReader.Create(Console.In);
        }

        #region Implementation of IStandardOut

        public IStandardStreamWriter Out { get; }
        public bool IsOutputRedirected => Console.IsOutputRedirected;

        #endregion

        #region Implementation of IStandardError

        public IStandardStreamWriter Error { get; }
        public bool IsErrorRedirected => Console.IsErrorRedirected;

        #endregion

        #region Implementation of IStandardIn

        public IStandardStreamReader In { get; }

        public bool IsInputRedirected => Console.IsInputRedirected;

        #endregion

        #region Implementation of IConsole

        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            return ((ConsoleKeyInfo)_ansiConsole.Input.ReadKey(intercept))!;
        }

        public bool TreatControlCAsInput
        {
            get => Console.TreatControlCAsInput;
            set => Console.TreatControlCAsInput = value;
        }

        #endregion
    }
}