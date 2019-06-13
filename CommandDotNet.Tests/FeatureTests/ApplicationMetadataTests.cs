using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using CommandDotNet.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class ApplicationMetadataTests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public ApplicationMetadataTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void App_BasicHelp_DisplaysApplicationMetadata()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "-h",
                Then = { Result = @"app description

Usage: SomeApp [options] [command]

Options:
  -h | --help  Show help information

Commands:
  somecommand  cmd description
  SubApp       sub-app description

Use ""SomeApp [command] --help"" for more information about a command.

app extended help" }
            });
        }

        [Fact]
        public void App_DetailedHelp_DisplaysApplicationMetadata()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "-h",
                Then = { Result = @"app description

Usage: SomeApp [options] [command]

Options:

  -h | --help
  Show help information


Commands:

  somecommand  cmd description
  SubApp       sub-app description

Use ""SomeApp [command] --help"" for more information about a command.

app extended help" }
            });
        }

        [Fact]
        public void NestedApp_BasicHelp_DisplaysApplicationMetadata()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "SubApp -h",
                Then = { Result = @"sub-app description

Usage: SomeApp SubApp [options] [command]

Options:
  -h | --help  Show help information

Commands:
  subdo

Use ""SomeApp SubApp [command] --help"" for more information about a command.

sub-app extended help" }
            });
        }

        [Fact]
        public void NestedApp_DetailedHelp_DisplaysApplicationMetadata()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "SubApp -h",
                Then = { Result = @"sub-app description

Usage: SomeApp SubApp [options] [command]

Options:

  -h | --help
  Show help information


Commands:

  subdo

Use ""SomeApp SubApp [command] --help"" for more information about a command.

sub-app extended help" }
            });
        }

        [Fact]
        public void Command_BasicHelp_DisplaysApplicationMetadata()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = BasicHelp },
                WhenArgs = "somecommand -h",
                Then = { Result = @"cmd description

Usage: SomeApp somecommand [arguments] [options]

Arguments:
  value

Options:
  -h | --help  Show help information

cmd extended help" }
            });
        }

        [Fact]
        public void Command_DetailedHelp_DisplaysApplicationMetadata()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DetailedHelp },
                WhenArgs = "somecommand -h",
                Then = { Result = @"cmd description

Usage: SomeApp somecommand [arguments] [options]

Arguments:

  value    <NUMBER>


Options:

  -h | --help
  Show help information


cmd extended help" }
            });
        }

        [Fact]
        public void Command_Exec_UsesNameFromApplicationMetadata()
        {
            Verify(new Given<App>
            {
                WhenArgs = $"somecommand 5",
                Then = { ExitCode = 5 }
            });
        }

        [Fact]
        public void NestedApp_Command_Exec_UsesNameFromApplicationMetadata()
        {
            Verify(new Given<App>
            {
                WhenArgs = $"SubApp subdo 5",
                Then = { ExitCode = 5 }
            });
        }

        [ApplicationMetadata(
            Description = "app description",
            Name = "SomeApp",
            ExtendedHelpText = "app extended help")]
        public class App
        {
            [ApplicationMetadata(
                Description = "cmd description",
                Name = "somecommand",
                ExtendedHelpText = "cmd extended help")]
            public int Do(int value)
            {
                return value;
            }

            [SubCommand]
            [ApplicationMetadata(
                Description = "sub-app description",
                Name = "SubApp",
                ExtendedHelpText = "sub-app extended help")]
            public class SubApp
            {
                [ApplicationMetadata(Name = "subdo")]
                public int Do(int value)
                {
                    return value;
                }
            }
        }
    }
}