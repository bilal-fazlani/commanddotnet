using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.TestTools;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
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
        public void WithParams_BasicHelp_IncludesArgsOtherCommands()
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
        public void WithParams_DetailedHelp_IncludesArgsOtherCommands()
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