using System.IO;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class FlagTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FlagTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        [Fact]
        public void TestValidFlagsWithDefaultSettings()
        {
            TestCaseRunner<ValidFlagsApplication> testCaseRunner = new TestCaseRunner<ValidFlagsApplication>(_testOutputHelper);
            testCaseRunner.Run("TestCases/FlagTests.TestValidFlagsWithDefaultSettings.Input.json",
                "TestCases/FlagTests.TestValidFlagsWithDefaultSettings.Output.json");
        }

        [Fact]
        public void TestValidFlagsWithExplicitBooleanMode()
        {
            TestCaseRunner<FlagAppForExplicitBooleanTest> testCaseRunner = 
                new TestCaseRunner<FlagAppForExplicitBooleanTest>(_testOutputHelper, new AppSettings
                {
                    BooleanMode = BooleanMode.Explicit
                });
            testCaseRunner.Run("TestCases/FlagTests.TestValidFlagsWithExplicitBooleanMode.Input.json",
                "TestCases/FlagTests.TestValidFlagsWithExplicitBooleanMode.Output.json");
        }
        
        [Fact]
        public void TestInvalidFlags()
        {
            AppRunner<ValidFlagsApplication> appRunner = new AppRunner<ValidFlagsApplication>();
            int exitCode = appRunner.Run(new[] {"CommandWithInvalidFlag"});
            exitCode.Should().Be(1);
        }
    }

    public class ValidFlagsApplication
    {
        public void CommandWithFlagTrueOverridden([Argument(BooleanMode = BooleanMode.Explicit)]bool flag)
        {
            string output = new
            {
                flag
            }.ToJson();
            
            File.WriteAllText("TestCases/FlagTests.TestValidFlagsWithDefaultSettings.Output.json",
                output);
        }
        
        public void CommandWithFlagFalseOverridden([Argument(BooleanMode = BooleanMode.Implicit)]bool flag)
        {
            string output = new
            {
                flag
            }.ToJson();
            
            File.WriteAllText("TestCases/FlagTests.TestValidFlagsWithDefaultSettings.Output.json",
                output);
        }

        public void CommandWithoutFlagOverride(bool flag)
        {
            string output = new
            {
                flag
            }.ToJson();
            
            File.WriteAllText("TestCases/FlagTests.TestValidFlagsWithDefaultSettings.Output.json",
                output);
        }
    }

    public class FlagAppForExplicitBooleanTest
    {
        public void CommandWithoutFlagOverride(bool flag)
        {
            string output = new
            {
                flag
            }.ToJson();
            
            File.WriteAllText("TestCases/FlagTests.TestValidFlagsWithExplicitBooleanMode.Output.json",
                output);
        }
    }

    public class InvalidFlagApplication
    {
        public void CommandWithInvalidFlag(string flag)
        {
        }
    }
}