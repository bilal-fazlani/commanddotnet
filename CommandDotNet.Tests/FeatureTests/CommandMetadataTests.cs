using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CommandMetadataTests : TestBase
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public CommandMetadataTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void App_BasicHelp_DisplaysCommandAttrData()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "-h",
                Then = { Result = @"app description

Usage: some usage examples

Commands:
  somecommand  cmd description
  SubApp       sub-app description

Use ""dotnet testhost.dll [command] --help"" for more information about a command.

app extended help" }
            });
        }

        [Fact]
        public void App_DetailedHelp_DisplaysCommandAttrData()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "-h",
                Then = { Result = @"app description

Usage: some usage examples

Commands:

  somecommand  cmd description
  SubApp       sub-app description

Use ""dotnet testhost.dll [command] --help"" for more information about a command.

app extended help" }
            });
        }

        [Fact]
        public void NestedApp_BasicHelp_DisplaysCommandAttrData()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "SubApp -h",
                Then = { Result = @"sub-app description

Usage: dotnet testhost.dll SubApp [command]

Commands:
  subdo

Use ""dotnet testhost.dll SubApp [command] --help"" for more information about a command.

sub-app extended help" }
            });
        }

        [Fact]
        public void NestedApp_DetailedHelp_DisplaysCommandAttrData()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "SubApp -h",
                Then = { Result = @"sub-app description

Usage: dotnet testhost.dll SubApp [command]

Commands:

  subdo

Use ""dotnet testhost.dll SubApp [command] --help"" for more information about a command.

sub-app extended help" }
            });
        }

        [Fact]
        public void Command_BasicHelp_DisplaysCommandAttrData()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = BasicHelp },
                WhenArgs = "somecommand -h",
                Then = { Result = @"cmd description

Usage: dotnet testhost.dll somecommand [arguments]

Arguments:
  value

cmd extended help" }
            });
        }

        [Fact]
        public void Command_DetailedHelp_DisplaysCommandAttrData()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = DetailedHelp },
                WhenArgs = "somecommand -h",
                Then = { Result = @"cmd description

Usage: dotnet testhost.dll somecommand [arguments]

Arguments:

  value  <NUMBER>

cmd extended help" }
            });
        }

        [Fact]
        public void Command_Exec_UsesNameFromCommandAttrData()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "somecommand 5",
                Then = { ExitCode = 5 }
            });
        }

        [Fact]
        public void NestedApp_Command_Exec_UsesNameFromCommandAttrData()
        {
            Verify(new Scenario<App>
            {
                WhenArgs = "SubApp subdo 5",
                Then = { ExitCode = 5 }
            });
        }

        // sanity check for ApplicationMetadata until it has been removed 
        [ApplicationMetadata(
            Description = "app description",
            Usage = "some usage examples",
            Name = "SomeApp",
            ExtendedHelpText = "app extended help")]
        public class App
        {
            [Command(
                Description = "cmd description",
                Name = "somecommand",
                ExtendedHelpText = "cmd extended help")]
            public int Do(int value)
            {
                return value;
            }

            [SubCommand]
            [Command(
                Description = "sub-app description",
                Name = "SubApp",
                ExtendedHelpText = "sub-app extended help")]
            public class SubApp
            {
                [Command(Name = "subdo")]
                public int Do(int value)
                {
                    return value;
                }
            }
        }
    }
}