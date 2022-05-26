// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using SysConsole=System.Console;

namespace CommandDotNet.Rendering
{
   // [CA1416] This call site is reachable on all platforms.
   // the warning does not seem to distinguish between getters and setters
   #pragma warning disable CA1416
   
    public class SystemConsole : IConsole
    {
        private const string Windows = "windows";

        public virtual string Title
        {
            [SupportedOSPlatform(Windows)]
            get => SysConsole.Title;
            set => SysConsole.Title = value;
        }

        public virtual Encoding InputEncoding
        {
            get => SysConsole.InputEncoding;
            set => SysConsole.InputEncoding = value;
        }

        public virtual Encoding OutputEncoding
        {
            get => SysConsole.OutputEncoding;
            set => SysConsole.OutputEncoding = value;
        }

        public virtual event ConsoleCancelEventHandler? CancelKeyPress
        {
            add => SysConsole.CancelKeyPress += value;
            remove => SysConsole.CancelKeyPress -= value;
        }

        #region IStandardError

        public virtual TextWriter Error => SysConsole.Error;

        public void SetError(TextWriter newError) => SysConsole.SetError(newError);

        public virtual bool IsErrorRedirected => SysConsole.IsErrorRedirected;

        #endregion

        #region IStandardOut

        public virtual TextWriter Out => SysConsole.Out;

        public void SetOut(TextWriter newOut) => SysConsole.SetOut(newOut);

        public virtual bool IsOutputRedirected => SysConsole.IsOutputRedirected;

        #endregion

        #region IStandardIn

        public virtual TextReader In => SysConsole.In;

        public void SetIn(TextReader newIn) => SysConsole.SetIn(newIn);

        public virtual bool IsInputRedirected => SysConsole.IsInputRedirected;

        #endregion

        #region IConsoleReader

        public virtual bool KeyAvailable => SysConsole.KeyAvailable;

        [SupportedOSPlatform(Windows)]
        public virtual bool NumberLock => SysConsole.NumberLock;

        [SupportedOSPlatform(Windows)]
        public virtual bool CapsLock => SysConsole.CapsLock;

        public virtual bool TreatControlCAsInput
        {
            get => SysConsole.TreatControlCAsInput;
            set => SysConsole.TreatControlCAsInput = value;
        }

        public virtual ConsoleKeyInfo? ReadKey(bool intercept = false)
        {
            return SysConsole.ReadKey(intercept);
        }

        public virtual int Read()
        {
            return SysConsole.In.Read();
        }

        public virtual string? ReadLine()
        {
            return SysConsole.In.ReadLine();
        }

        #endregion

        #region IConsoleColor

        public virtual ConsoleColor BackgroundColor
        {
            get => SysConsole.BackgroundColor;
            set => SysConsole.BackgroundColor = value;
        }

        public virtual ConsoleColor ForegroundColor
        {
            get => SysConsole.ForegroundColor;
            set => SysConsole.ForegroundColor = value;
        }

        public virtual void ResetColor() => SysConsole.ResetColor();

        #endregion

        #region IConsoleBuffer

        public virtual int BufferWidth
        {
            get => SysConsole.BufferWidth;
            [SupportedOSPlatform(Windows)]
            set => SysConsole.BufferWidth = value;
        }

        public virtual int BufferHeight
        {
            get => SysConsole.BufferHeight;
            [SupportedOSPlatform(Windows)]
            set => SysConsole.BufferHeight = value;
        }

        [SupportedOSPlatform(Windows)]
        public virtual void SetBufferSize(int width, int height) => SysConsole.SetBufferSize(width, height);

        public virtual void Clear() => SysConsole.Clear();

        #endregion

        #region IConsoleWindow

        public virtual int WindowLeft
        {
            get => SysConsole.WindowLeft;
            [SupportedOSPlatform(Windows)]
            set => SysConsole.WindowLeft = value;
        }

        public virtual int WindowTop
        {
            get => SysConsole.WindowTop;
            [SupportedOSPlatform(Windows)]
            set => SysConsole.WindowTop = value;
        }

        public virtual int WindowWidth
        {
            get => SysConsole.WindowWidth;
            [SupportedOSPlatform(Windows)]
            set => SysConsole.WindowWidth = value;
        }

        public virtual int WindowHeight
        {
            get => SysConsole.WindowHeight;
            [SupportedOSPlatform(Windows)]
            set => SysConsole.WindowHeight = value;
        }

        [SupportedOSPlatform(Windows)]
        public virtual void SetWindowPosition(int left, int top) => SysConsole.SetWindowPosition(left, top);

        [SupportedOSPlatform(Windows)]
        public virtual void SetWindowSize(int width, int height) => SysConsole.SetBufferSize(width, height);

        #endregion

        #region IConsoleCursor

        public virtual int CursorSize
        {
            get => SysConsole.CursorSize;
            [SupportedOSPlatform(Windows)]
            set => SysConsole.CursorSize = value;
        }

        public virtual bool CursorVisible
        {
            [SupportedOSPlatform(Windows)]
            get => SysConsole.CursorVisible;
            set => SysConsole.CursorVisible = value;
        }

        public virtual int CursorLeft
        {
            get => SysConsole.CursorLeft;
            set => SysConsole.CursorLeft = value;
        }

        public virtual int CursorTop
        {
            get => SysConsole.CursorTop;
            set => SysConsole.CursorTop = value;
        }

        public virtual void SetCursorPosition(int left, int top) => SysConsole.SetCursorPosition(left, top);

        #endregion
    }
}