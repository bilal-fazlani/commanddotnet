using CommandDotNet.Tests.Utils;
using FluentAssertions;

namespace CommandDotNet.Tests
{
    public class AppRunnerResult
    {
        public int ExitCode { get; }
        public string ConsoleOut { get; }

        public AppRunnerResult(int exitCode, string consoleOut)
        {
            this.ExitCode = exitCode;
            this.ConsoleOut = consoleOut;
        }

        /// <summary>
        /// Help generation leaves extra trailing spaces that are hard to account for in test verification.
        /// This method removes trailing white space from each line and standardizes Environment.NewLine
        /// for all line endings
        /// </summary>
        /// <param name="expected"></param>
        public void HelpShouldBe(string expected)
        {
            var actual = ConsoleOut.NormalizeLineEndings();
            expected = expected.NormalizeLineEndings();
            actual.Should().Be(expected);
        }
    }
}