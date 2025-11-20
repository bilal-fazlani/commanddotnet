using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments;

public class ArgumentGroupTests
{
    public ArgumentGroupTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
    public void BasicHelp_Groups_Options_With_Group_Property()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "GroupedOptions -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll GroupedOptions [options]

Options:
  --ungrouped  An ungrouped option

Database:

  --connection  Connection string
  --timeout     Timeout value

Logging:

  --logLevel  Log level
  --logFile   Log file path"
                    }
                });
    }

    [Fact]
    public void DetailedHelp_Groups_Options_With_Group_Property()
    {
        new AppRunner<App>(TestAppSettings.DetailedHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "GroupedOptions -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll GroupedOptions [options]

Options:

  --ungrouped  <TEXT>
  An ungrouped option

Database:

  --connection  <TEXT>
  Connection string

  --timeout     <NUMBER>
  Timeout value

Logging:

  --logLevel  <TEXT>
  Log level

  --logFile   <TEXT>
  Log file path"
                    }
                });
    }

    [Fact]
    public void Groups_Are_Sorted_Alphabetically()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "AlphabeticalGroups -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll AlphabeticalGroups [options]

Options:
  --ungrouped  Ungrouped option

Alpha:

  --alphaOpt  Alpha option

Beta:

  --betaOpt  Beta option

Zeta:

  --zetaOpt  Zeta option"
                    }
                });
    }

    [Fact]
    public void ArgumentModel_With_ArgumentGroupAttribute_Groups_All_Properties()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "ModelWithGroup -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll ModelWithGroup [options]

Options:
  --ungrouped  Ungrouped option

Server Settings:

  --Host  Server host
  --Port  Server port"
                    }
                });
    }

    [Fact]
    public void PropertyLevel_Group_Overrides_Model_Group()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "ModelWithPropertyOverride -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll ModelWithPropertyOverride [options]

Options:
  --ungrouped  Ungrouped option

Database:

  --Connection  Connection string

Server Settings:

  --Host  Server host"
                    }
                });
    }

    [Fact]
    public void Nested_Models_Inherit_Parent_Group()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "NestedModelsInheritGroup -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll NestedModelsInheritGroup [options]

Server Settings:

  --Host     Server host
  --Port     Server port
  --Timeout  Connection timeout
  --Retries  Connection retries"
                    }
                });
    }

    [Fact]
    public void Nested_Model_Can_Override_Parent_Group()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "NestedModelOverridesGroup -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll NestedModelOverridesGroup [options]

Advanced:

  --Timeout  Advanced timeout
  --Retries  Advanced retries

Server Settings:

  --Host  Server host
  --Port  Server port"
                    }
                });
    }

    [Fact]
    public void Operands_Are_Not_Grouped()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "MixedOptionsAndOperands -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll MixedOptionsAndOperands [options] <file>

Arguments:
  file  Input file

Options:
  --ungrouped  Ungrouped option

Processing:

  --threads  Thread count
  --memory   Memory limit"
                    }
                });
    }

    [Fact]
    public void Multiple_Options_In_Same_Group()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "MultipleOptionsPerGroup -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll MultipleOptionsPerGroup [options]

Security:

  --apiKey       API key
  --apiSecret    API secret
  --apiEndpoint  API endpoint"
                    }
                });
    }

    private class App
    {
        public void GroupedOptions(
            [Option(Description = "An ungrouped option")] string ungrouped,
            [Option(Group = "Database", Description = "Connection string")] string connection,
            [Option(Group = "Database", Description = "Timeout value")] int timeout,
            [Option(Group = "Logging", Description = "Log level")] string logLevel,
            [Option(Group = "Logging", Description = "Log file path")] string logFile)
        { }

        public void AlphabeticalGroups(
            [Option(Description = "Ungrouped option")] string ungrouped,
            [Option(Group = "Zeta", Description = "Zeta option")] string zetaOpt,
            [Option(Group = "Alpha", Description = "Alpha option")] string alphaOpt,
            [Option(Group = "Beta", Description = "Beta option")] string betaOpt)
        { }

        public void ModelWithGroup(
            [Option(Description = "Ungrouped option")] string ungrouped,
            ServerSettingsModel settings)
        { }

        public void ModelWithPropertyOverride(
            [Option(Description = "Ungrouped option")] string ungrouped,
            ServerSettingsWithOverrideModel settings)
        { }

        public void NestedModelsInheritGroup(ServerSettingsWithNestedModel settings)
        { }

        public void NestedModelOverridesGroup(ServerSettingsWithNestedOverrideModel settings)
        { }

        public void MixedOptionsAndOperands(
            [Option(Description = "Ungrouped option")] string ungrouped,
            [Option(Group = "Processing", Description = "Thread count")] int threads,
            [Option(Group = "Processing", Description = "Memory limit")] int memory,
            [Operand(Description = "Input file")] string file)
        { }

        public void MultipleOptionsPerGroup(
            [Option(Group = "Security", Description = "API key")] string apiKey,
            [Option(Group = "Security", Description = "API secret")] string apiSecret,
            [Option(Group = "Security", Description = "API endpoint")] string apiEndpoint)
        { }
    }

    [ArgumentGroup("Server Settings")]
    private class ServerSettingsModel : IArgumentModel
    {
        [Option(Description = "Server host")]
        public string? Host { get; set; }

        [Option(Description = "Server port")]
        public int Port { get; set; }
    }

    [ArgumentGroup("Server Settings")]
    private class ServerSettingsWithOverrideModel : IArgumentModel
    {
        [Option(Description = "Server host")]
        public string? Host { get; set; }

        [Option(Group = "Database", Description = "Connection string")]
        public string? Connection { get; set; }
    }

    [ArgumentGroup("Server Settings")]
    private class ServerSettingsWithNestedModel : IArgumentModel
    {
        [Option(Description = "Server host")]
        public string? Host { get; set; }

        [Option(Description = "Server port")]
        public int Port { get; set; }

        public ConnectionSettingsModel? Connection { get; set; }
    }

    // Nested model without ArgumentGroup - inherits from parent
    private class ConnectionSettingsModel : IArgumentModel
    {
        [Option(Description = "Connection timeout")]
        public int Timeout { get; set; }

        [Option(Description = "Connection retries")]
        public int Retries { get; set; }
    }

    [ArgumentGroup("Server Settings")]
    private class ServerSettingsWithNestedOverrideModel : IArgumentModel
    {
        [Option(Description = "Server host")]
        public string? Host { get; set; }

        [Option(Description = "Server port")]
        public int Port { get; set; }

        public AdvancedSettingsModel? Advanced { get; set; }
    }

    // Nested model with its own ArgumentGroup - overrides parent
    [ArgumentGroup("Advanced")]
    private class AdvancedSettingsModel : IArgumentModel
    {
        [Option(Description = "Advanced timeout")]
        public int Timeout { get; set; }

        [Option(Description = "Advanced retries")]
        public int Retries { get; set; }
    }
}

