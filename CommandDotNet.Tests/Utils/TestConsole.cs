// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied & adapted from System.CommandLine 

using System.IO;
using System.Text;
using CommandDotNet.Rendering;

namespace CommandDotNet.Tests.Utils
{
    public class TestConsole : IConsole
    {
        public TestConsole()
        {
            var joined = new StandardStreamWriter();
            Joined = joined;
            Out = new StandardStreamWriter(joined);
            Error = new StandardStreamWriter(joined);
        }

        public IStandardStreamWriter Error { get; protected set; }

        public IStandardStreamWriter Out { get; protected set; }

        public IStandardStreamWriter Joined { get; protected set; }

        public bool IsOutputRedirected { get; protected set; }

        public bool IsErrorRedirected { get; protected set; }

        public bool IsInputRedirected { get; protected set; }

        internal class StandardStreamWriter : TextWriter, IStandardStreamWriter
        {
            private readonly StandardStreamWriter _inner;
            private readonly StringBuilder _stringBuilder = new StringBuilder();

            public StandardStreamWriter(StandardStreamWriter inner = null)
            {
                _inner = inner;
            }

            public override void Write(char value)
            {
                _inner?.Write(value);
                _stringBuilder.Append(value);
            }

            public override Encoding Encoding { get; } = Encoding.Unicode;

            public override string ToString() => _stringBuilder.ToString();
        }
    }
}