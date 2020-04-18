using System;
using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ResponseFileTests : IDisposable
    {
        private readonly TempFiles _tempFiles;

        public ResponseFileTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
            _tempFiles = new TempFiles(output.WriteLine);
        }

        public void Dispose()
        {
            _tempFiles.Dispose();
        }

        [Fact]
        public void WhenFeatureDisabled_ArgumentsAreNotReadFromResponseFile()
        {
            var responseFile = _tempFiles.CreateTempFile("--opt1 opt1value arg1value");

            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = $"Do @{responseFile}" },
                    Then =
                    {
                        Captured =
                        {
                            new DoResult {arg1 = $"@{responseFile}"}
                        }
                    }
                });
        }

        [Fact]
        public void ArgumentsAreReadFromResponseFile()
        {
            var responseFile = _tempFiles.CreateTempFile("--opt1 opt1value arg1value");

            new AppRunner<App>()
                .UseResponseFiles()
                .Verify(new Scenario
                {
                    When = { Args = $"Do @{responseFile}" },
                    Then =
                    {
                        Captured =
                        {
                            new DoResult
                            {
                                arg1 = "arg1value",
                                opt1 = "opt1value"
                            }
                        }
                    }
                });
        }

        [Fact]
        public void ArgumentsInResponseFileCanBeSplitAcrossLines()
        {
            var responseFile = _tempFiles.CreateTempFile("--opt1 opt1value", "arg1value");

            new AppRunner<App>()
                .UseResponseFiles()
                .Verify(new Scenario
                {
                    When = { Args = $"Do @{responseFile}" },
                    Then =
                    {
                        Captured =
                        {
                            new DoResult
                            {
                                arg1 = "arg1value",
                                opt1 = "opt1value"
                            }
                        }
                    }
                });
        }

        [Fact]
        public void ArgumentsCanBeSpecifiedInResponseFileAndCommandLine()
        {
            var responseFile = _tempFiles.CreateTempFile("--opt1 opt1value");

            new AppRunner<App>()
                .UseResponseFiles()
                .Verify(new Scenario
                {
                    When = { Args = $"Do arg1value @{responseFile}" },
                    Then =
                    {
                        Captured =
                        {
                            new DoResult
                            {
                                arg1 = "arg1value",
                                opt1 = "opt1value"
                            }
                        }
                    }
                });
        }

        [Fact]
        public void Operand_ValueBeginningWithAmpersand_CanBeEscapedWith_EndOfOptions_ArgumentSeparator()
        {
            var endOfOptions = new AppSettings{DefaultArgumentSeparatorStrategy = ArgumentSeparatorStrategy.EndOfOptions};
            new AppRunner<App>(endOfOptions)
                .UseResponseFiles()
                .Verify(new Scenario
                {
                    When = {Args = "Do -- @some-value"},
                    Then =
                    {
                        Captured = {new DoResult {arg1 = "@some-value"}},
                        AssertContext = c =>
                            c.ParseResult.SeparatedArguments.Should().Contain("@some-value")
                    }
                });
        }

        [Fact]
        public void Option_ValueBeginningWithAmpersand_CanBeEscapedWith_ColonValueAssignment()
        {
            new AppRunner<App>()
                .UseResponseFiles()
                .Verify(new Scenario
                {
                    When = { Args = "Do --opt1:@some-value" },
                    Then =
                    {
                        Captured = {new DoResult {opt1 = "@some-value"}}
                    }
                });
        }

        [Fact]
        public void Option_ValueBeginningWithAmpersand_CanBeEscapedWith_EqualsValueAssignment()
        {
            new AppRunner<App>()
                .UseResponseFiles()
                .Verify(new Scenario
                {
                    When = { Args = "Do --opt1=@some-value" },
                    Then =
                    {
                        Captured = {new DoResult {opt1 = "@some-value"}}
                    }
                });
        }

        [Fact]
        public void WhenFileNotFound_Exit_Code1_And_InformativeErrorMessage_And_LogError()
        {
            List<string> logs = new List<string>();

            new AppRunner<App>()
                .UseResponseFiles()
                .Verify(new Scenario
                {
                    When = { Args = "Do @not-exists.rsp" },
                    Then =
                    {
                        ExitCode = 1,
                        AssertOutput = o =>
                        {
                            o.Should().StartWith("File not found: ");
                            o.TrimEnd(Environment.NewLine.ToCharArray())
                                .Should().EndWith("not-exists.rsp");
                        }
                    }
                }, logLine: line =>
                {
                    logs.Add(line);
                    Ambient.Output.WriteLine(line);
                }, TestConfig.Default.Where(c => c.PrintCommandDotNetLogs = true));

            // the E indicates error level log
            logs.Should().Contain(log =>
                log.StartsWith(
                    "E CommandDotNet.Tokens.TokenizerPipeline > Tokenizer error: TokenTransformation: expand-response-files")
                && log.Contains("File not found")
                && log.Contains("not-exists.rsp"));
        }

        private class App
        {
            private TestCaptures TestCaptures { get; set; }

            public void Do([Option] string opt1, string arg1)
            {
                TestCaptures.Capture(new DoResult { opt1 = opt1, arg1 = arg1 });
            }
        }

        public class DoResult
        {
            public string opt1;
            public string arg1;
        }
    }
}