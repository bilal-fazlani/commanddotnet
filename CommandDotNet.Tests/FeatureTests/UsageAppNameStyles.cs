using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class UsageAppNameStyles : ScenarioTestBase<UsageAppNameStyles>
    {
        public UsageAppNameStyles(ITestOutputHelper output) : base(output)
        {
        }

        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<WithAppMetadataName>("Adaptive style uses GlobalTool style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.Adaptive}}},
                    WhenArgs = "-h",
                    Then = { ResultsContainsTexts = { "Usage: AppName" }}
                },
                new Given<WithAppMetadataName>("GlobalTool style uses GlobalTool style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.GlobalTool}}},
                    WhenArgs = "-h",
                    Then = { ResultsContainsTexts = { "Usage: AppName" }}
                },
                new Given<WithAppMetadataName>("DotNet style uses DotNet style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.DotNet}}},
                    WhenArgs = "-h",
                    Then = { ResultsContainsTexts = { "Usage: dotnet testhost.dll" } }
                },
                new Given<WithAppMetadataName>("Executable style uses Executable style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.Executable}}},
                    WhenArgs = "-h",
                    Then = { ResultsContainsTexts = { "Usage: testhost.dll" } }
                },

                new Given<WithoutAppMetadatName>("Adaptive style falls back to DotNet style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.Adaptive}}},
                    WhenArgs = "-h",
                    Then = { ResultsContainsTexts = { "Usage: dotnet testhost.dll" } }
                },
                new Given<WithoutAppMetadatName>("GlobalTool style throws configuration exception")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.GlobalTool}}},
                    WhenArgs = "-h",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Invalid configuration: ApplicationMetadataAttribute.Name is required for the root command when UsageAppNameStyle.GlobalTool is specified." }
                    }
                },
                new Given<WithoutAppMetadatName>("DotNet style uses DotNet style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.DotNet}}},
                    WhenArgs = "-h",
                    Then = { ResultsContainsTexts = { "Usage: dotnet testhost.dll" } }
                },
                new Given<WithoutAppMetadatName>("Executable style uses Executable style")
                {
                    And = { AppSettings = new AppSettings{Help = {UsageAppNameStyle = UsageAppNameStyle.Executable}}},
                    WhenArgs = "-h",
                    Then = { ResultsContainsTexts = { "Usage: testhost.dll" } }
                },
            };

        [ApplicationMetadata(Name = "AppName")]
        public class WithAppMetadataName
        {

        }

        public class WithoutAppMetadatName
        {

        }
    }
}