using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class UsageAppNameStyles : TestBase
    {
        public UsageAppNameStyles(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AdaptiveStyleUsesGlobalToolStyle()
        {
            Verify(new Given<WithAppMetadataName>
            {
                And = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.Adaptive } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: AppName" } }
            });
        }

        [Fact]
        public void GlobalToolStyleUsesGlobalToolStyle()
        {
            Verify(new Given<WithAppMetadataName>
            {
                And = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.GlobalTool } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: AppName" } }
            });
        }

        [Fact]
        public void DotNetStyleUsesDotNetStyle()
        {
            Verify(new Given<WithoutAppMetadatName>
            {
                And = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.DotNet } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: dotnet testhost.dll" } }
            });
        }

        [Fact]
        public void ExecutableStyleUsesExecutableStyle()
        {
            Verify(new Given<WithAppMetadataName>
            {
                And = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.Executable } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: testhost.dll" } }
            });
        }

        [Fact]
        public void AdaptiveStyleFallsBackToDotNetStyle()
        {
            Verify(new Given<WithoutAppMetadatName>
            {
                And = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.Adaptive } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: dotnet testhost.dll" } }
            });
        }

        [Fact]
        public void GlobalToolStyleThrowsConfigurationException()
        {
            Verify(new Given<WithoutAppMetadatName>
            {
                And = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.GlobalTool } } },
                WhenArgs = "-h",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { "Invalid configuration: ApplicationMetadataAttribute.Name is required for the root command when UsageAppNameStyle.GlobalTool is specified." }
                }
            });
        }

        [ApplicationMetadata(Name = "AppName")]
        public class WithAppMetadataName
        {

        }

        public class WithoutAppMetadatName
        {

        }
    }
}