using FluentAssertions;

namespace CommandDotNet.TestTools
{
    public class AppRunnerResult
    {
        public int ExitCode { get; }
        public string ConsoleOut { get; }
        public TestOutputs TestOutputs { get; }

        public AppRunnerResult(int exitCode, string consoleOut, TestOutputs testOutputs)
        {
            ExitCode = exitCode;
            ConsoleOut = consoleOut;
            TestOutputs = testOutputs;
        }

        /// <summary>
        /// Help generation leaves extra trailing spaces that are hard to account for in test verification.
        /// This method removes trailing white space from each line and standardizes Environment.NewLine
        /// for all line endings
        /// </summary>
        public void OutputShouldBe(string expected)
        {
            var actual = ConsoleOut.NormalizeLineEndings();
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
            var actual = ConsoleOut.NormalizeLineEndings();
            expected = expected.NormalizeLineEndings();
            return actual.Contains(expected);
        }
    }
}