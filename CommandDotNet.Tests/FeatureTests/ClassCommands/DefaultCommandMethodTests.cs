using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class DefaultCommandMethodTests
    {
        private readonly ITestOutputHelper _output;
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;
        
        public DefaultCommandMethodTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void WithoutDefaultArgs_BasicHelp_IncludesOtherCommands()
        {
            new AppRunner<WithoutDefaultArgsApp>(BasicHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "-h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll [command]

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void WithoutDefaultArgs_DetailedHelp_IncludesOtherCommands()
        {
            new AppRunner<WithoutDefaultArgsApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "-h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll [command]

Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_BasicHelp_IncludesArgsAndOtherCommands()
        {
            new AppRunner<WithDefaultArgsApp>(BasicHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "-h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll [command] [arguments]

Arguments:
  text  some text

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_DetailedHelp_IncludesArgsAndOtherCommands()
        {
            new AppRunner<WithDefaultArgsApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "-h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll [command] [arguments]

Arguments:

  text  <TEXT>
  some text

Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }


        [Fact]
        public void WithDefaultArgs_Help_ForAnotherCommand_DoesNotIncludeDefaultArgs()
        {
            new AppRunner<WithDefaultArgsApp>(DetailedHelp).VerifyScenario(_output, new Scenario
            {
                WhenArgs = "AnotherCommand -h",
                Then =
                {
                    Output = @"Usage: dotnet testhost.dll AnotherCommand"
                }
            });
        }

        [Fact]
        public void WithoutDefaultArgs_Execute_works()
        {
            new AppRunner<WithoutDefaultArgsApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = null,
                Then =
                {
                    Captured = { WithoutDefaultArgsApp.DefaultMethodExecuted }
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_Execute_works()
        {
            new AppRunner<WithDefaultArgsApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "abcde",
                Then =
                {
                    Captured = { "abcde" }
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_Execute_AnotherCommand_WorksWithoutParams()
        {
            new AppRunner<WithDefaultArgsApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "AnotherCommand",
                Then =
                {
                    Captured = { false }
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_Execute_AnotherCommand_FailsWithDefaultParams()
        {
            new AppRunner<WithDefaultArgsApp>().VerifyScenario(_output, new Scenario
            {
                WhenArgs = "AnotherCommand abcde",
                Then =
                {
                    ExitCode = 1,
                    OutputContainsTexts = { "Unrecognized command or argument 'abcde'"}
                }
            });
        }

        private class WithoutDefaultArgsApp
        {
            public const string DefaultMethodExecuted = "default executed";

            private TestCaptures TestCaptures { get; set; }

            [DefaultMethod]
            public void DefaultMethod()
            {
                TestCaptures.Capture(DefaultMethodExecuted);
            }

            public void AnotherCommand()
            {
                TestCaptures.Capture(false);
            }
        }

        private class WithDefaultArgsApp
        {
            private TestCaptures TestCaptures { get; set; }

            [DefaultMethod]
            public void DefaultMethod(
                [Operand(Description = "some text")]
                string text)
            {
                TestCaptures.Capture(text);
            }

            public void AnotherCommand()
            {
                TestCaptures.Capture(false);
            }
        }
    }
}