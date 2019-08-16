// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied & adapted from System.CommandLine 

using System;
using System.IO;
using System.Text;
using CommandDotNet.Rendering;

namespace CommandDotNet.TestTools
{
    public class TestConsole : IConsole
    {
        public TestConsole(
            Func<TestConsole, string> onReadLine = null,
            Func<TestConsole, string> onReadToEnd = null)
        {
            IsInputRedirected = onReadToEnd != null;

            var joined = new StandardStreamWriter();
            Joined = joined;
            Out = new StandardStreamWriter(joined);
            Error = new StandardStreamWriter(joined);
            In = new StandardStreamReader(
                () =>
                {
                    var input = onReadLine?.Invoke(this);
                    // write to joined output so it can be logged for debugging
                    joined.WriteLine();
                    joined.WriteLine($"IConsole.ReadLine > {input}");
                    joined.WriteLine();
                    return input;
                },
                () =>
                {
                    var input = onReadToEnd?.Invoke(this);
                    // write to joined output so it can be logged for debugging
                    joined.WriteLine();
                    joined.WriteLine($"IConsole.ReadToEnd > {input}");
                    joined.WriteLine();
                    return input;
                });
        }

        public IStandardStreamWriter Error { get; }

        public IStandardStreamWriter Out { get; }

        public IStandardStreamWriter Joined { get; }

        public bool IsOutputRedirected { get; }

        public bool IsErrorRedirected { get; }

        public IStandardStreamReader In { get; }

        public bool IsInputRedirected { get; }

        internal class StandardStreamReader : IStandardStreamReader
        {
            private readonly Func<string> _onReadLine;
            private readonly Func<string> _onReadToEnd;

            public StandardStreamReader(Func<string> onReadLine, Func<string> onReadToEnd)
            {
                _onReadLine = onReadLine;
                _onReadToEnd = onReadToEnd;
            }

            public string ReadLine()
            {
                return _onReadLine?.Invoke();
            }

            public string ReadToEnd()
            {
                return _onReadToEnd?.Invoke();
            }
        }

        internal class StandardStreamWriter : TextWriter, IStandardStreamWriter
        {
            private readonly StandardStreamWriter _inner;
            private readonly StringBuilder _stringBuilder = new StringBuilder();

            public StandardStreamWriter(
                StandardStreamWriter inner = null)
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