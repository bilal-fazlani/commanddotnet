using Spectre.Console;
using Spectre.Console.Testing;

namespace CommandDotNet.Spectre.Testing
{
    /// <summary>
    /// Contains extensions for <see cref="AnsiTestConsole"/> to delegate to Spectre's <see cref="TestConsole"/>
    /// </summary>
    public static class TestConsoleExtensions
    {
        /// <summary>
        /// Sets the console's color system.
        /// </summary>
        /// <param name="console">The console.</param>
        /// <param name="colors">The color system to use.</param>
        /// <returns>The same instance so that multiple calls can be chained.</returns>
        public static AnsiTestConsole Colors(this AnsiTestConsole console, ColorSystem colors)
        {
            console.SpectreTestConsole.Colors(colors);
            return console;
        }

        /// <summary>
        /// Sets whether or not ANSI is supported.
        /// </summary>
        /// <param name="console">The console.</param>
        /// <param name="enable">Whether or not VT/ANSI control codes are supported.</param>
        /// <returns>The same instance so that multiple calls can be chained.</returns>
        public static AnsiTestConsole SupportsAnsi(this AnsiTestConsole console, bool enable)
        {
            console.SpectreTestConsole.SupportsAnsi(enable);
            return console;
        }

        /// <summary>
        /// Makes the console interactive.
        /// </summary>
        /// <param name="console">The console.</param>
        /// <returns>The same instance so that multiple calls can be chained.</returns>
        public static AnsiTestConsole Interactive(this AnsiTestConsole console)
        {
            console.SpectreTestConsole.Interactive();
            return console;
        }

        /// <summary>
        /// Sets the console width.
        /// </summary>
        /// <param name="console">The console.</param>
        /// <param name="width">The console width.</param>
        /// <returns>The same instance so that multiple calls can be chained.</returns>
        public static AnsiTestConsole Width(this AnsiTestConsole console, int width)
        {
            console.SpectreTestConsole.Width(width);
            return console;
        }

        /// <summary>
        /// Turns on emitting of VT/ANSI sequences.
        /// </summary>
        /// <param name="console">The console.</param>
        /// <returns>The same instance so that multiple calls can be chained.</returns>
        public static AnsiTestConsole EmitAnsiSequences(this AnsiTestConsole console)
        {
            console.SpectreTestConsole.EmitAnsiSequences();
            return console;
        }
    }
}