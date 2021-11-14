using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.TestTools
{
    internal class TestConsoleReader : TextReader
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly TestConsole _host;
        private readonly Queue<char> _currentLine = new();
        private Func<ITestConsole, string?>? _onReadLine;

        public TestConsoleReader(TestConsole host)
        {
            _host = host;
        }

        public override int Read()
        {
            if (!_currentLine.Any())
            {
                var line = _onReadLine?.Invoke(_host);
                if (line is null) return -1;
                line.ForEach(_currentLine.Enqueue);
            }
            return _currentLine.Dequeue();
        }

        public override string? ReadLine()
        {
            if (_currentLine.Any())
            {
                var remainingLine = new string(_currentLine.ToArray());
                _currentLine.Clear();
                return remainingLine;
            }
            return _onReadLine?.Invoke(_host);
        }

        private bool IsInputRedirected
        {
            get => _host.IsInputRedirected;
            set => _host.IsInputRedirected = value;
        }

        public void Mock(IEnumerable<string> pipedInput, bool overwrite = false)
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
                _onReadLine = _ => queue.Count == 0 ? null : queue.Dequeue();
            }
            else
            {
                // take one at a time
                _onReadLine = _ => pipedInput.Take(1).FirstOrDefault();
            }

            var onReadLine = _onReadLine;

            _onReadLine = _ =>
            {
                var input = onReadLine?.Invoke(_host);
                Log.Info($"ITestConsole.ReadLine > {input}");
                return input;
            };
        }

        public void Mock(Func<ITestConsole, string?> onReadLine, bool overwrite = false)
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

            _onReadLine = _ =>
            {
                var input = onReadLine?.Invoke(_host);
                Log.Info($"ITestConsole.ReadLine > {input}");
                return input;
            };
        }
    }
}