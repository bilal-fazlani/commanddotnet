// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// copied from System.CommandLine 

using System;
using System.Text;
using CommandDotNet.Rendering;

namespace CommandDotNet
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
}