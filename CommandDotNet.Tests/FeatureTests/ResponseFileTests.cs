using System;
using System.IO;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using CommandDotNet.Tokens;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class ResponseFileTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly TempFiles _tempFiles;

        public ResponseFileTests(ITestOutputHelper output)
        {
            _output = output;
            _tempFiles = new TempFiles(_output.AsLogger());
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
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = $"Do @{responseFile}",
                    Then =
                    {
                        Outputs =
                        {
                            new App.DoResult {arg1 = $"@{responseFile}"}
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
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = $"Do @{responseFile}",
                    Then =
                    {
                        Outputs =
                        {
                            new App.DoResult
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
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = $"Do @{responseFile}",
                    Then =
                    {
                        Outputs =
                        {
                            new App.DoResult
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
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = $"Do arg1value @{responseFile}",
                    Then =
                    {
                        Outputs =
                        {
                            new App.DoResult
                            {
                                arg1 = "arg1value",
                                opt1 = "opt1value"
                            }
                        }
                    }
                });
        }

        // TEST only Arguments, not Directives or Separated

        [Fact]
        public void OutputsClearErrorWhenFileNotFound()
        {
            var appRunner = new AppRunner<App>().UseResponseFiles();
            var ex = Assert.Throws<TokenTransformationException>(() => 
                appRunner.Run("Do", "@not-exists"));

            ex.Message.Should().Contain("expand-response-file");
            ex.InnerException.Should().BeOfType<FileNotFoundException>();
            var fileName = ((FileNotFoundException)ex.InnerException).FileName;
            Path.GetFileName(fileName).Should().Be("not-exists");
        }

        public class App
        {
            private TestOutputs TestOutputs { get; set; }

            public void Do([Option] string opt1, string arg1)
            {
                TestOutputs.Capture(new DoResult { opt1 = opt1, arg1 = arg1 });
            }

            public class DoResult
            {
                public string opt1;
                public string arg1;
            }
        }
    }
}