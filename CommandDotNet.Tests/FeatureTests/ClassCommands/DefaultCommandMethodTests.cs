using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class DefaultCommandMethodTests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;
        
        public DefaultCommandMethodTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void WithoutDefaultArgs_BasicHelp_IncludesOtherCommands()
        {
            Verify(new Scenario<WithoutDefaultArgsApp>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [command]

Commands:
  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void WithoutDefaultArgs_DetailedHelp_IncludesOtherCommands()
        {
            Verify(new Scenario<WithoutDefaultArgsApp>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [command]

Commands:

  AnotherCommand

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_BasicHelp_IncludesArgsAndOtherCommands()
        {
            Verify(new Scenario<WithDefaultArgsApp>
            {
                Given = {AppSettings = BasicHelp},
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [command] [arguments]

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
            Verify(new Scenario<WithDefaultArgsApp>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "-h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll [command] [arguments]

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
            Verify(new Scenario<WithDefaultArgsApp>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "AnotherCommand -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll AnotherCommand"
                }
            });
        }

        [Fact]
        public void WithoutDefaultArgs_Execute_works()
        {
            Verify(new Scenario<WithoutDefaultArgsApp>
            {
                WhenArgs = null,
                Then =
                {
                    Outputs = { WithoutDefaultArgsApp.DefaultMethodExecuted }
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_Execute_works()
        {
            Verify(new Scenario<WithDefaultArgsApp>
            {
                WhenArgs = "abcde",
                Then =
                {
                    Outputs = { "abcde" }
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_Execute_AnotherCommand_WorksWithoutParams()
        {
            Verify(new Scenario<WithDefaultArgsApp>
            {
                WhenArgs = "AnotherCommand",
                Then =
                {
                    Outputs = { false }
                }
            });
        }

        [Fact]
        public void WithDefaultArgs_Execute_AnotherCommand_FailsWithDefaultParams()
        {
            Verify(new Scenario<WithDefaultArgsApp>
            {
                WhenArgs = "AnotherCommand abcde",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { "Unrecognized command or argument 'abcde'"}
                }
            });
        }

        public class WithoutDefaultArgsApp
        {
            public const string DefaultMethodExecuted = "default executed";

            private TestOutputs TestOutputs { get; set; }

            [DefaultMethod]
            public void DefaultMethod()
            {
                TestOutputs.Capture(DefaultMethodExecuted);
            }

            public void AnotherCommand()
            {
                TestOutputs.Capture(false);
            }
        }

        public class WithDefaultArgsApp
        {
            private TestOutputs TestOutputs { get; set; }

            [DefaultMethod]
            public void DefaultMethod(
                [Operand(Description = "some text")]
                string text)
            {
                TestOutputs.Capture(text);
            }

            public void AnotherCommand()
            {
                TestOutputs.Capture(false);
            }
        }
    }
}