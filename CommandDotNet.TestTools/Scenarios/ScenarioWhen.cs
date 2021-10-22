using System;
using System.Collections.Generic;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools.Prompts;

namespace CommandDotNet.TestTools.Scenarios
{
    public class ScenarioWhen
    {
        /// <summary>
        /// The input as would be typed in the shell.
        /// Can handle quoted strings. 
        /// </summary>
        public string? Args { get; set; }

        /// <summary>Use this for tested arguments that can contain spaces</summary>
        public string[]? ArgsArray { get; set; }

        /// <summary>
        /// Use this delegate to mimic input in response to a <see cref="IConsole"/>.In.ReadLine()<br/>
        /// Use <see cref="ITestConsole"/>.Out.ToString() to get the output up to that point.
        /// </summary>
        public Func<ITestConsole, string>? OnReadLine { get; set; }

        /// <summary>
        /// Use this delegate to mimic input in response to a <see cref="IConsole"/>.ReadKey()<br/>
        /// Use <see cref="ITestConsole"/>.Out.ToString() to get the output up to that point.
        /// </summary>
        public IPromptResponder? OnPrompt { get; set; }

        /// <summary>Use to mimic piped input from the shell.</summary>
        public IEnumerable<string>? PipedInput { get; set; }
    }
}