using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help;

public class DescriptionMethodTests
{
    public DescriptionMethodTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
    public void DescriptionMethod_Works_For_Option()
    {
        new AppRunner<DescriptionMethodApp>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "WithDescriptionMethod -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll WithDescriptionMethod [options]

Options:
  --target  Available targets: web, api, worker"
                    }
                });
    }

    [Fact]
    public void DescriptionMethod_Works_For_Operand()
    {
        new AppRunner<DescriptionMethodApp>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "WithOperandDescriptionMethod -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll WithOperandDescriptionMethod <environment>

Arguments:
  environment  Available environments: dev, staging, prod"
                    }
                });
    }

    [Fact]
    public void DescriptionMethod_Error_When_Multiple_Descriptions_Set()
    {
        new AppRunner<DescriptionMethodDupeApp>().Verify(new Scenario
        {
            When = { Args = "" },
            Then =
            {
                ExitCode = 1,
                Output = "CommandDotNet.InvalidConfigurationException: Multiple description properties were set for " +
                         "CommandDotNet.Tests.FeatureTests.Help.DescriptionMethodTests+DescriptionMethodDupeApp.Do.option: Description, DescriptionMethodAttribute. Only one can be set."
            }
        });
    }

    [Fact]
    public void DescriptionMethod_Error_When_Method_Not_Found()
    {
        new AppRunner<DescriptionMethodNotFoundApp>().Verify(new Scenario
        {
            When = { Args = "" },
            Then =
            {
                ExitCode = 1,
                Output = "CommandDotNet.InvalidConfigurationException: DescriptionMethod 'NonExistentMethod' not found in type 'DescriptionMethodNotFoundApp'. " +
                         "Method must be static with no parameters and return string. " +
                         "Defined on CommandDotNet.Tests.FeatureTests.Help.DescriptionMethodTests+DescriptionMethodNotFoundApp.Do.option"
            }
        });
    }

    [Fact]
    public void DescriptionMethod_Error_When_Method_Wrong_Return_Type()
    {
        new AppRunner<DescriptionMethodWrongReturnTypeApp>().Verify(new Scenario
        {
            When = { Args = "" },
            Then =
            {
                ExitCode = 1,
                Output = "CommandDotNet.InvalidConfigurationException: DescriptionMethod 'GetDescriptionWrongType' in type 'DescriptionMethodWrongReturnTypeApp' must return string but returns Int32. " +
                         "Defined on CommandDotNet.Tests.FeatureTests.Help.DescriptionMethodTests+DescriptionMethodWrongReturnTypeApp.Do.option"
            }
        });
    }

    private class DescriptionMethodApp
    {
        public void WithDescriptionMethod(
            [Option, DescriptionMethod(nameof(GetTargetDescription))] string target) { }

        public void WithOperandDescriptionMethod(
            [Operand, DescriptionMethod(nameof(GetEnvironmentDescription))] string environment) { }

        private static string GetTargetDescription() => "Available targets: web, api, worker";
        private static string GetEnvironmentDescription() => "Available environments: dev, staging, prod";
    }

    private class DescriptionMethodDupeApp
    {
        public void Do([Option(Description = "static description"), DescriptionMethod(nameof(GetDescription))] string option) { }
        private static string GetDescription() => "dynamic description";
    }

    private class DescriptionMethodNotFoundApp
    {
        public void Do([Option, DescriptionMethod("NonExistentMethod")] string option) { }
    }

    private class DescriptionMethodWrongReturnTypeApp
    {
        public void Do([Option, DescriptionMethod(nameof(GetDescriptionWrongType))] string option) { }
        private static int GetDescriptionWrongType() => 42;
    }
}
