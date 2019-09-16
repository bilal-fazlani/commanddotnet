using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class DefaultCommandMethodTests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp.Clone(a => a.EnableVersionOption = false);
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp.Clone(a => a.EnableVersionOption = false);
        
        public DefaultCommandMethodTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void WithoutParams_BasicHelp_IncludesOtherCommands()
        {
            Verify(new Scenario<WithoutParamsApp>
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
        public void WithoutParams_DetailedHelp_IncludesOtherCommands()
        {
            Verify(new Scenario<WithoutParamsApp>
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
        public void WithParams_BasicHelp_IncludesArgsAndOtherCommands()
        {
            Verify(new Scenario<WithParamsApp>
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
        public void WithParams_DetailedHelp_IncludesArgsAndOtherCommands()
        {
            Verify(new Scenario<WithParamsApp>
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


        [Fact(Skip = "requires framework to distinguish interceptor options from default options")]
        public void WithParams_Help_ForAnotherCommand_DoesNotIncludeDefaultArgs()
        {
            Verify(new Scenario<WithParamsApp>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "AnotherCommand -h",
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
        public void WithoutParams_Execute_works()
        {
            Verify(new Scenario<WithoutParamsApp>
            {
                WhenArgs = null,
                Then =
                {
                    Outputs = { WithoutParamsApp.DefaultMethodExecuted }
                }
            });
        }

        [Fact]
        public void WithParams_Execute_works()
        {
            Verify(new Scenario<WithParamsApp>
            {
                WhenArgs = "abcde",
                Then =
                {
                    Outputs = { "abcde" }
                }
            });
        }

        [Fact]
        public void WithParams_Execute_AnotherCommand_WorksWithoutParams()
        {
            Verify(new Scenario<WithParamsApp>
            {
                WhenArgs = "AnotherCommand",
                Then =
                {
                    Outputs = { false }
                }
            });
        }

        [Fact(Skip = "requires framework to distinguish interceptor options from default options")]
        public void WithParams_Execute_AnotherCommand_FailsWithDefaultParams()
        {
            Verify(new Scenario<WithParamsApp>
            {
                WhenArgs = "AnotherCommand abcde",
                Then =
                {
                    ResultsContainsTexts = { "Unrecognized command or argument 'abcde'"}
                }
            });
        }

        public class WithoutParamsApp
        {
            public const string DefaultMethodExecuted = "default executed";

            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

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

        public class WithParamsApp
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

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