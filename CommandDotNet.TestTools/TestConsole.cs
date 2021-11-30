// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied & adapted from System.CommandLine 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandDotNet.Prompts;

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
        private readonly TestConsoleWriter _allOutput;
        private Func<ITestConsole, ConsoleKeyInfo>? _onReadKey;

        public static class Defaults
        {
            public static ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
            public static ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;

            public static int WindowLeft { get; set; } = 0;
            public static int WindowTop { get; set; } = 0;
            public static int WindowWidth { get; set; } = 120;
            public static int WindowHeight { get; set; } = 30;

            public static int BufferWidth { get; set; } = WindowWidth;
            public static int BufferHeight { get; set; } = WindowHeight;
        }

        public TestConsole(bool trimEnd = true)
        {
            _allOutput = new TestConsoleWriter(trimEnd: trimEnd);
            Out = new TestConsoleWriter(_allOutput, trimEnd);
            Error = new TestConsoleWriter(_allOutput, trimEnd);
            In = new TestConsoleReader(this);
        }

        public virtual ITestConsole Mock(IEnumerable<string> pipedInput, bool overwrite = false)
        {
            ((TestConsoleReader)In).Mock(pipedInput, overwrite);
            return this;
        }

        public virtual ITestConsole Mock(Func<ITestConsole, string?> onReadLine, bool overwrite = false)
        {
            ((TestConsoleReader)In).Mock(onReadLine, overwrite);
            return this;
        }

        public virtual ITestConsole Mock(Func<ITestConsole, ConsoleKeyInfo>? onReadKey, bool overwrite = false)
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
        /// The combination of <see cref="Console.Error"/> and <see cref="Console.Out"/>
        /// in the order they were written from the app.<br/>
        /// This is how the output would appear in the shell.
        /// </summary>
        public virtual string? AllText() => _allOutput.ToString();

        /// <summary>
        /// The accumulated text of the <see cref="Console.Out"/> stream.
        /// </summary>
        public virtual string? OutText() => Out.ToString();

        /// <summary>
        /// The accumulated text of the <see cref="Console.Error"/> stream.
        /// </summary>
        public virtual string? ErrorText() => Error.ToString();

        #region IConsole

        public virtual string Title { get; set; } = "Test";
        public virtual Encoding InputEncoding { get; set; } = Encoding.Unicode;
        public virtual Encoding OutputEncoding { get; set; } = Encoding.Unicode;
        
        public virtual ConsoleKeyInfo? ReadKey(bool intercept = false)
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

        #endregion

        #region IStandardError

        public virtual TextWriter Error { get; }

        public void SetError(TextWriter newError)
        {
            ((TestConsoleWriter)Error).Replaced = newError;
        }

        public virtual bool IsErrorRedirected { get; } = false;

        #endregion

        #region IStandardOut

        public virtual TextWriter Out { get; }

        public void SetOut(TextWriter newOut)
        {
            ((TestConsoleWriter)Out).Replaced = newOut;
        }

        public virtual bool IsOutputRedirected { get; } = false;

        #endregion

        #region IStandardIn

        public virtual TextReader In { get; private set; }

        public void SetIn(TextReader newIn)
        {
            In = newIn;
        }

        public virtual bool IsInputRedirected { get; set; }

        #endregion

        #region IConsoleReader

        public virtual int Read() => In.Read();

        public virtual string? ReadLine() => In.ReadLine();

        #endregion

        #region IConsoleKeys

        public virtual bool KeyAvailable { get; set; }
        public virtual bool NumberLock { get; set; }
        public virtual bool CapsLock { get; set; }
        public virtual bool TreatControlCAsInput { get; set; }
        public virtual event ConsoleCancelEventHandler? CancelKeyPress;

        #endregion

        #region IConsoleColor

        public virtual ConsoleColor BackgroundColor { get; set; } = Defaults.BackgroundColor;
        public virtual ConsoleColor ForegroundColor { get; set; } = Defaults.ForegroundColor;

        public virtual void ResetColor()
        {
            BackgroundColor = Defaults.BackgroundColor;
            ForegroundColor = Defaults.ForegroundColor;
        }

        #endregion

        #region IConsoleBuffer

        public virtual int BufferWidth { get; set; } = Defaults.BufferWidth;
        public virtual int BufferHeight { get; set; } = Defaults.BufferHeight;

        public virtual void SetBufferSize(int width, int height)
        {
            BufferWidth = width;
            BufferHeight = height;
        }

        public virtual void Clear()
        {

        }

        #endregion

        #region IConsoleWindow

        public virtual int WindowLeft { get; set; } = Defaults.WindowLeft;
        public virtual int WindowTop { get; set; } = Defaults.WindowTop;
        public virtual int WindowWidth { get; set; } = Defaults.WindowWidth;
        public virtual int WindowHeight { get; set; } = Defaults.WindowHeight;

        public virtual void SetWindowPosition(int left, int top)
        {
            WindowLeft = left;
            WindowTop = top;
        }

        public virtual void SetWindowSize(int width, int height)
        {
            WindowWidth = width;
            WindowHeight = height;
        }

        #endregion

        #region IConsoleCursor

        public virtual int CursorSize { get; set; }
        public virtual bool CursorVisible { get; set; }
        public virtual int CursorLeft { get; set; }
        public virtual int CursorTop { get; set; }

        public virtual void SetCursorPosition(int left, int top)
        {
            CursorLeft = left;
            CursorTop = top;
        }

        #endregion
    }
}