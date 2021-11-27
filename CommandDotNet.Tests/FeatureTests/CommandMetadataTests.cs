using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CommandMetadataTests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public CommandMetadataTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void App_BasicHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"app description

Usage: some usage examples

Commands:
  Multiline    descr1
descr2
  somecommand  cmd description
  SubApp       sub-app description

Use ""testhost.dll [command] --help"" for more information about a command.

app extended help"
                }
            });
        }

        [Fact]
        public void App_DetailedHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"app description

Usage: some usage examples

Commands:

  Multiline    descr1
descr2
  somecommand  cmd description
  SubApp       sub-app description

Use ""testhost.dll [command] --help"" for more information about a command.

app extended help"
                }
            });
        }

        [Fact]
        public void NestedApp_BasicHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "SubApp -h"},
                Then =
                {
                    Output = @"sub-app description

Usage: testhost.dll SubApp [command]

Commands:
  subdo

Use ""testhost.dll SubApp [command] --help"" for more information about a command.

sub-app extended help"
                }
            });
        }

        [Fact]
        public void NestedApp_DetailedHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "SubApp -h"},
                Then =
                {
                    Output = @"sub-app description

Usage: testhost.dll SubApp [command]

Commands:

  subdo

Use ""testhost.dll SubApp [command] --help"" for more information about a command.

sub-app extended help"
                }
            });
        }

        [Fact]
        public void Command_BasicHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "somecommand -h"},
                Then =
                {
                    Output = @"cmd description

Usage: testhost.dll somecommand <value>

Arguments:
  value

cmd extended help"
                }
            });
        }

        [Fact]
        public void Command_DetailedHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "somecommand -h"},
                Then =
                {
                    Output = @"cmd description

Usage: testhost.dll somecommand <value>

Arguments:

  value  <NUMBER>

cmd extended help"
                }
            });
        }

        [Fact]
        public void Command_Exec_UsesNameFromCommandAttrData()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "somecommand 5"},
                Then = { ExitCode = 5 }
            });
        }

        [Fact]
        public void NestedApp_Command_Exec_UsesNameFromCommandAttrData()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "SubApp subdo 5"},
                Then = { ExitCode = 5 }
            });
        }

        [Fact]
        public void Multiline_BasicHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = { Args = "Multiline -h" },
                Then =
                {
                    Output = @"descr1
descr2

Usage: usage1
usage2

exthelp1
exthelp2"
                }
            });
        }

        [Fact]
        public void Multiline_DetailedHelp_DisplaysCommandAttrData()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = { Args = "Multiline -h" },
                Then =
                {
                    Output = @"descr1
descr2

Usage: usage1
usage2

exthelp1
exthelp2"
                }
            });
        }

        [Fact]
        public void Errors_when_duplicate_description_properties_are_used()
        {
            new AppRunner<DupeDescriptions>().Verify(new Scenario
            {
                When = { Args = "" },
                Then =
                {
                    ExitCode = 1,
                    Output = "CommandDotNet.InvalidConfigurationException: Both Description and DescriptionLines were set for " +
                             "CommandDotNet.Tests.FeatureTests.CommandMetadataTests+DupeDescriptions. Only one can be set."
                }
            });
        }

        [Fact]
        public void Errors_when_duplicate_usage_properties_are_used()
        {
            new AppRunner<DupeUsages>().Verify(new Scenario
            {
                When = { Args = "" },
                Then =
                {
                    ExitCode = 1,
                    Output = "CommandDotNet.InvalidConfigurationException: Both Usage and UsageLines were set for " +
                             "CommandDotNet.Tests.FeatureTests.CommandMetadataTests+DupeUsages. Only one can be set."
                }
            });
        }

        [Fact]
        public void Errors_when_duplicate_extendedhelp_properties_are_used()
        {
            new AppRunner<DupeExtendendHelps>().Verify(new Scenario
            {
                When = { Args = "" },
                Then =
                {
                    ExitCode = 1,
                    Output = "CommandDotNet.InvalidConfigurationException: Both ExtendedHelpText and ExtendedHelpTextLines were set for " +
                             "CommandDotNet.Tests.FeatureTests.CommandMetadataTests+DupeExtendendHelps. Only one can be set."
                }
            });
        }

        [Command(Description = "", DescriptionLines = new[] { "" })]
        private class DupeDescriptions { }

        [Command(Usage = "", UsageLines = new[] { "" })]
        private class DupeUsages { }

        [Command(ExtendedHelpText = "", ExtendedHelpTextLines = new[] { "" })]
        private class DupeExtendendHelps { }

        // sanity check for ApplicationMetadata until it has been removed 
        [Command("SomeApp",
            Description = "app description",
            Usage = "some usage examples",
            ExtendedHelpText = "app extended help")]
        private class App
        {
            [Command("somecommand",
                Description = "cmd description",
                ExtendedHelpText = "cmd extended help")]
            public int Do(int value)
            {
                return value;
            }

            [Subcommand]
            [Command("SubApp",
                Description = "sub-app description",
                ExtendedHelpText = "sub-app extended help")]
            public class SubApp
            {
                [Command("subdo")]
                public int Do(int value)
                {
                    return value;
                }
            }

            [Command(
                DescriptionLines = new[] { "descr1", "descr2" },
                UsageLines = new[] { "usage1", "usage2" },
                ExtendedHelpTextLines = new[] { "exthelp1", "exthelp2" })]
            public void Multiline()
            {

            }
        }
    }
}
