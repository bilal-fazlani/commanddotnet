// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;

namespace CommandDotNet.Rendering
{
    public class SystemConsole : IConsole
    {
        public SystemConsole()
        {
            Error = StandardStreamWriter.Create(Console.Error);
            Out = StandardStreamWriter.Create(Console.Out);
            In = StandardStreamReader.Create(Console.In);
        }

        public IStandardStreamWriter Error { get; }

        public bool IsErrorRedirected => Console.IsErrorRedirected;

        public IStandardStreamWriter Out { get; }

        public bool IsOutputRedirected => Console.IsOutputRedirected;

        public IStandardStreamReader In { get; }

        public bool IsInputRedirected => Console.IsInputRedirected;

        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            return Console.ReadKey(intercept);
        }

        public bool TreatControlCAsInput
        {
            get => Console.TreatControlCAsInput;
            set => Console.TreatControlCAsInput = value;
        }
    }
}