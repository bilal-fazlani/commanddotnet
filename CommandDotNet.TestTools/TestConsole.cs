// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied & adapted from System.CommandLine 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;
using CommandDotNet.Prompts;
using CommandDotNet.Rendering;

namespace CommandDotNet.TestTools
{
    /// <summary>
    /// A test console that can be used to <br/>
    /// - capture output <br/>
    /// - provide piped input <br/>
    /// - handle ReadLine and ReadToEnd requests
    /// </summary>
    public class TestConsole : ITestConsole
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private Func<ITestConsole, ConsoleKeyInfo>? _onReadKey;
        private Func<ITestConsole, string?>? _onReadLine;

        public TestConsole()
        {
            var all = new StandardStreamWriter();
            All = all;
            Out = new StandardStreamWriter(all);
            Error = new StandardStreamWriter(all);
            In = new StandardStreamReader();
        }

        public ITestConsole Mock(IEnumerable<string> pipedInput, bool overwrite = false)
        {
            if (pipedInput == null)
            {
                throw new ArgumentNullException(nameof(pipedInput));
            }

            if (IsInputRedirected)
            {
                if (overwrite)
                {
                    _onReadLine = null;
                }
                else
                {
                    throw new Exception($"{nameof(pipedInput)} has already been mocked");
                }
            }

            if (_onReadLine != null)
            {
                throw new Exception("onReadLine and pipedInput cannot both be specified. " +
                                    "Windows will throw 'System.IO.IOException: The handle is invalid' on an attempt to ");
            }
            
            IsInputRedirected = true;

            if (pipedInput is ICollection<string> inputs)
            {
                var queue = new Queue<string>(inputs);
                _onReadLine = console => queue.Count == 0 ? null : queue.Dequeue();
            }
            else
            {
                // take one at a time
                _onReadLine = console => pipedInput.Take(1).FirstOrDefault();
            }

            In = new StandardStreamReader(
                () =>
                {
                    var input = _onReadLine?.Invoke(this);
                    Log.Info($"ITestConsole.ReadLine > {input}");
                    return input;
                });

            return this;
        }

        public ITestConsole Mock(Func<ITestConsole, string?> onReadLine, bool overwrite = false)
        {
            if (onReadLine == null)
            {
                throw new ArgumentNullException(nameof(onReadLine));
            }

            if (_onReadLine != null)
            {
                if (overwrite)
                {
                    _onReadLine = null;
                }
                else
                {
                    throw new Exception($"{nameof(onReadLine)} has already been mocked");
                }
            }

            if (IsInputRedirected)
            {
                throw new Exception("onReadLine and pipedInput cannot both be specified. " +
                                    "Windows will throw 'System.IO.IOException: The handle is invalid' on an attempt to ");
            }

            _onReadLine = onReadLine;

            In = new StandardStreamReader(
                () =>
                {
                    var input = _onReadLine?.Invoke(this);
                    Log.Info($"ITestConsole.ReadLine > {input}");
                    return input;
                });
            return this;
        }

        public ITestConsole Mock(Func<ITestConsole, ConsoleKeyInfo>? onReadKey, bool overwrite = false)
        {
            if (_onReadKey != null)
            {
                if (overwrite)
                {
                    _onReadKey = null;
                }
                else
                {
                    throw new Exception($"{nameof(onReadKey)} has already been mocked");
                }
            }

            _onReadKey = onReadKey;
            return this;
        }

        /// <summary>
        /// This is the combined output for <see cref="Error"/> and <see cref="Out"/> in the order the lines were output.
        /// </summary>
        public IStandardStreamWriter All { get; private set; }

        public IStandardStreamWriter Out { get; private set; }

        public IStandardStreamWriter Error { get; private set; }

        /// <summary>
        /// The combination of <see cref="Console.Error"/> and <see cref="Console.Out"/>
        /// in the order they were written from the app.<br/>
        /// This is how the output would appear in the shell.
        /// </summary>
        public string AllText() => All.ToString();

        /// <summary>
        /// The accumulated text of the <see cref="Console.Out"/> stream.
        /// </summary>
        public string OutText() => Out.ToString();

        /// <summary>
        /// The accumulated text of the <see cref="Console.Error"/> stream.
        /// </summary>
        public string ErrorText() => Error.ToString();

        public bool IsOutputRedirected { get; } = false;

        public bool IsErrorRedirected { get; } = false;

        public IStandardStreamReader In { get; private set; }

        public bool IsInputRedirected { get; private set; }

        /// <summary>
        /// Read a key from the input
        /// </summary>
        /// <param name="intercept">When true, the key is not displayed in the output</param>
        /// <returns></returns>
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            ConsoleKeyInfo consoleKeyInfo;

            do
            {
                consoleKeyInfo = _onReadKey?.Invoke(this)
                                 ?? new ConsoleKeyInfo('\u0000', ConsoleKey.Enter, false, false, false);

                // mimic System.Console which does not interrupt during ReadKey
                // and does not return Ctrl+C unless TreatControlCAsInput == true.
            } while (!TreatControlCAsInput && consoleKeyInfo.IsCtrlC());

            if (!intercept)
            {
                if (consoleKeyInfo.Key == ConsoleKey.Enter)
                {
                    Out.WriteLine("");
                }
                else
                {
                    Out.Write(consoleKeyInfo.KeyChar.ToString());
                }
            }
            return consoleKeyInfo;
        }

        public bool TreatControlCAsInput { get; set; }

        private class StandardStreamReader : IStandardStreamReader
        {
            private readonly Func<string?>? _onReadLine;

            public StandardStreamReader(Func<string?>? onReadLine = null)
            {
                _onReadLine = onReadLine;
            }

            public string? ReadLine()
            {
                return _onReadLine?.Invoke();
            }

            public string? ReadToEnd()
            {
                return _onReadLine?.EnumeratePipedInput().ToCsv(Environment.NewLine);
            }
        }

        private class StandardStreamWriter : TextWriter, IStandardStreamWriter
        {
            private readonly StandardStreamWriter? _inner;
            private readonly StringBuilder _stringBuilder = new StringBuilder();

            public StandardStreamWriter(
                StandardStreamWriter? inner = null)
            {
                _inner = inner;
            }

            public override void Write(char value)
            {
                _inner?.Write(value);
                if (value == '\b' && _stringBuilder.Length > 0)
                {
                    _stringBuilder.Length = _stringBuilder.Length - 1;
                }
                else
                {
                    _stringBuilder.Append(value);
                }
            }

            public override Encoding Encoding { get; } = Encoding.Unicode;

            public override string ToString() => _stringBuilder.ToString();
        }
    }
}