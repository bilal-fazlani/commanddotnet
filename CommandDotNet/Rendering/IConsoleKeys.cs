using System;

namespace CommandDotNet.Rendering
{
    public interface IConsoleKeys
    {
        /// <summary>Gets a value indicating whether a key press is available in the input stream.</summary>
        bool KeyAvailable { get; }

        /// <summary>Gets a value indicating whether the NUM LOCK keyboard toggle is turned on or turned off.</summary>
        bool NumberLock { get; }

        /// <summary>Gets a value indicating whether the CAPS LOCK keyboard toggle is turned on or turned off.</summary>
        bool CapsLock { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the combination of the
        /// <see cref="ConsoleModifiers"/>.Control  modifier key and
        /// <see cref="ConsoleKey"/>.C console key (Ctrl+C) is treated as
        /// ordinary input or as an interruption that is handled by the operating system.
        /// </summary>
        /// <returns>true if Ctrl+C is treated as ordinary input; otherwise, false.</returns>
        bool TreatControlCAsInput { get; set; }

        /// <summary>
        /// Occurs when the <see cref="ConsoleModifiers"/>.Control modifier key (Ctrl) and either
        /// the <see cref="ConsoleKey"/>.C console key (C) or the Break key are pressed simultaneously
        /// (Ctrl+C or Ctrl+Break)
        /// </summary>
        event ConsoleCancelEventHandler? CancelKeyPress;
    }
}