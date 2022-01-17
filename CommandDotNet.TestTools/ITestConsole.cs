using System;
using System.Collections.Generic;

namespace CommandDotNet.TestTools
{
    public interface ITestConsole: IConsole
    {
        /// <summary>
        /// The combination of <see cref="Console.Error"/> and <see cref="Console.Out"/>
        /// in the order they were written from the app.<br/>
        /// This is how the output would appear in the shell.
        /// </summary>
        string? AllText();

        /// <summary>
        /// The accumulated text of the <see cref="Console.Out"/> stream.
        /// </summary>
        string? OutText();

        /// <summary>
        /// The accumulated text of the <see cref="Console.Error"/> stream.
        /// </summary>
        string? ErrorText();

        ITestConsole Mock(IEnumerable<string> pipedInput, bool overwrite = false);
        ITestConsole Mock(Func<ITestConsole, string?> onReadLine, bool overwrite = false);
        ITestConsole Mock(Func<ITestConsole, ConsoleKeyInfo>? onReadKey, bool overwrite = false);
    }
}