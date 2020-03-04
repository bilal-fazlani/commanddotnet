using CommandDotNet.Help;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
    public class UsageAppNameStylesTests : TestBase
    {
        public UsageAppNameStylesTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GlobalToolStyleUsesGlobalToolStyle()
        {
            Verify(new Scenario<WithAppMetadataName>
            {
                Given = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.GlobalTool } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: AppName" } }
            });
        }

        [Fact]
        public void DotNetStyleUsesDotNetStyle()
        {
            Verify(new Scenario<WithoutAppMetadatName>
            {
                Given = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.DotNet } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: dotnet testhost.dll" } }
            });
        }

        [Fact]
        public void ExecutableStyleUsesExecutableStyle()
        {
            Verify(new Scenario<WithAppMetadataName>
            {
                Given = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.Executable } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: testhost.dll" } }
            });
        }

        [Fact]
        public void AdaptiveStyleFallsBackToDotNetStyle()
        {
            Verify(new Scenario<WithoutAppMetadatName>
            {
                Given = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.Adaptive } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: dotnet testhost.dll" } }
            });
        }

        [Fact]
        public void GlobalToolStyleThrowsConfigurationException()
        {
            Verify(new Scenario<WithoutAppMetadatName>
            {
                Given = { AppSettings = new AppSettings { Help = { UsageAppNameStyle = UsageAppNameStyle.GlobalTool } } },
                WhenArgs = "-h",
                Then =
                {
                    ExitCode = 1,
                    ResultsContainsTexts = { $"Invalid configuration: {nameof(CommandAttribute)}.{nameof(CommandAttribute.Name)} is required " +
                                             $"for the root command when {nameof(UsageAppNameStyle)}.{nameof(UsageAppNameStyle.GlobalTool)} is specified." }
                }
            });
        }

        [Fact]
        public void UsageAppNameTemplate_Should_ReplaceTemplateIn_Description_ExtendendHelp_UsageOverride()
        {
            Verify(new Scenario<UsageAppNameTemplate>
            {
                WhenArgs = "-h",
                Then = { ResultsContainsTexts =
                {
                    "descr dotnet testhost.dll",
                    "use dotnet testhost.dll",
                    "ext dotnet testhost.dll"
                } }
            });
        }

        [Fact]
        public void UsageAppNameSettingUsedWhenProvided()
        {
            Verify(new Scenario<WithAppMetadataName>
            {
                Given = { AppSettings = new AppSettings { Help = { UsageAppName = "WhatATool" } } },
                WhenArgs = "-h",
                Then = { ResultsContainsTexts = { "Usage: WhatATool" } }
            });
        }

        [Command(Name = "AppName")]
        public class WithAppMetadataName
        {

        }

        public class WithoutAppMetadatName
        {

        }

        [Command(
            Description = "descr %UsageAppName%",
            Usage = "use %UsageAppName%",
            ExtendedHelpText = "ext %UsageAppName%")]
        public class UsageAppNameTemplate
        {

        }
    }
}