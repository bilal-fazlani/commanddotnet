// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;
using System.IO;
using System.Text;

namespace CommandDotNet.Rendering
{
    public interface IConsole :
        IStandardError, IStandardIn, IStandardOut, IConsoleKeys,
        IConsoleBuffer, IConsoleColor, IConsoleCursor, IConsoleWindow
    {
        string Title { get; set; }
        Encoding InputEncoding { get; set; }
        Encoding OutputEncoding { get; set; }

        /// <summary>
        /// Obtains the next character or function key pressed by the user.
        /// The pressed key is optionally displayed in the console window.
        /// </summary>
        /// <param name="intercept">When true, the key is not displayed in the output</param>
        /// <returns>
        /// An object that describe the <see cref="ConsoleKey"/> constant and Unicode character,
        /// if any, that correspond to the pressed console key. The <see cref="ConsoleKeyInfo"/>
        /// object also describes, in a bitwise combination of <see cref="ConsoleModifiers"/> values,
        /// whether one or more Shift, Alt or Ctrl modifier keys was pressed simultaneously with
        /// the console key.
        /// </returns>
        ConsoleKeyInfo? ReadKey(bool intercept = false);
    }

    public interface IStandardError
    {
        /// <summary>Gets the standard error output stream.</summary>
        TextWriter Error { get; }

        void SetError(TextWriter newError);

        /// <summary>Gets a value that indicates whether the error output stream has been redirected from the standard error stream.</summary>
        bool IsErrorRedirected { get; }
    }

    public interface IStandardIn
    {
        TextReader In { get; }
        
        void SetIn(TextReader newIn);

        bool IsInputRedirected { get; }
    }

    public interface IStandardOut
    {
        TextWriter Out { get; }
        
        void SetOut(TextWriter newOut);

        bool IsOutputRedirected { get; }
    }

    public interface IConsoleBuffer
    {
        int BufferWidth { get; set; }
        int BufferHeight { get; set; }
        void SetBufferSize(int width, int height);
        void Clear();
    }

    public interface IConsoleColor
    {
        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }
        void ResetColor();
    }

    public interface IConsoleCursor
    {
        int CursorSize { get; set; }
        bool CursorVisible { get; set; }
        int CursorLeft { get; set; }
        int CursorTop { get; set; }
        void SetCursorPosition(int left, int top);
    }

    public interface IConsoleWindow
    {
        int WindowLeft { get; set; }
        int WindowTop { get; set; }
        int WindowWidth { get; set; }
        int WindowHeight { get; set; }
        void SetWindowPosition(int left, int top);
        void SetWindowSize(int width, int height);
    }
}