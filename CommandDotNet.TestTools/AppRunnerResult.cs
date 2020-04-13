using System;
using FluentAssertions;

namespace CommandDotNet.TestTools
{
    public class AppRunnerResult
    {
        internal AppRunner Runner { get; }
        internal TestConfig Config { get; }

        public int ExitCode { get; }

        public TestConsole Console { get; }

        /// <summary>
        /// <see cref="TestCaptures"/> captured in the command class.
        /// The command class must have a public <see cref="TestCaptures"/> property for this to work.<br/>
        /// This is a convenience for testing how inputs are mapped into the command method parameters.<br/>
        /// Useful for testing middleware components, not the business logic of your commands.
        /// </summary>
        public TestCaptures TestCaptures { get; }

        /// <summary>The <see cref="CommandContext"/> used during the run</summary>
        public CommandContext CommandContext { get; }

        /// <summary>
        /// The exception that escaped from <see cref="AppRunner.Run"/><br/>
        /// Can only populated when <see cref="TestConfig.OnErrorConfig.CaptureAndReturnResult"/> is true.
        /// </summary>
        public Exception EscapedException { get; }

        public AppRunnerResult(int exitCode, AppRunner runner,
            CommandContext commandContext, TestConsole testConsole, TestCaptures testCaptures,
            TestConfig config, Exception escapedException = null)
        {
            ExitCode = exitCode;
            Runner = runner;
            CommandContext = commandContext;
            Console = testConsole;
            TestCaptures = testCaptures;
            Config = config;
            EscapedException = escapedException;
        }

        /// <summary>
        /// Help generation leaves extra trailing spaces that are hard to account for in test verification.
        /// This method removes trailing white space from each line and standardizes Environment.NewLine
        /// for all line endings
        /// </summary>
        public void OutputShouldBe(string expected)
        {
            var actual = Console.AllText(normalizeLineEndings:true);
            expected = expected.NormalizeLineEndings();
            actual.Should().Be(expected);
        }

        /// <summary>
        /// Help generation leaves extra trailing spaces that are hard to account for in test verification.
        /// This method removes trailing white space from each line and standardizes Environment.NewLine
        /// for all line endings
        /// </summary>
        public bool OutputContains(string expected)
        {
            var actual = Console.AllText(normalizeLineEndings: true);
            expected = expected.NormalizeLineEndings();
            return actual.Contains(expected);
        }

        /// <summary>
        /// Help generation leaves extra trailing spaces that are hard to account for in test verification.
        /// This method removes trailing white space from each line and standardizes Environment.NewLine
        /// for all line endings
        /// </summary>
        public bool OutputNotContains(string expected)
        {
            return !OutputContains(expected);
        }
    }
}