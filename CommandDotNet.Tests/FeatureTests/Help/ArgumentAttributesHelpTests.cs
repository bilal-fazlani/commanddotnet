using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help;

public class ArgumentAttributesHelpTests
{
    public ArgumentAttributesHelpTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
    public void BasicHelp_Includes_Description()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll Do [options] <operand>

Arguments:
  operand  operand-descr

Options:
  --option  option-descr"
                    }
                });
    }

    [Fact]
    public void DetailedHelp_Includes_Description()
    {
        new AppRunner<App>(TestAppSettings.DetailedHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "Do -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll Do [options] <operand>

Arguments:

  operand  <TEXT>
  operand-descr

Options:

  --option  <TEXT>
  option-descr"
                    }
                });
    }

    [Fact]
    public void BasicHelp_Includes_Multiline_Description()
    {
        new AppRunner<App>(TestAppSettings.BasicHelp)
            .Verify(
                new Scenario
                {
                    When = { Args = "Multiline -h" },
                    Then =
                    {
                        Output = @"Usage: testhost.dll Multiline [options] <operand>

Arguments:
  operand  y_descr1
y_descr2

Options:
  --option  x_descr1
x_descr2"
                    }
                });
    }

    [Fact]
    public void DetailedHelp_Includes_Multiline_Description()
    {
        new AppRunner<App>(TestAppSettings.DetailedHelp)
            .Verify(
                new Scenario
                {
                    When = { Args = "Multiline -h" },
                    Then =
                    {
                        Output = @"Usage: testhost.dll Multiline [options] <operand>

Arguments:

  operand  <NUMBER>
  y_descr1
y_descr2

Options:

  --option  <NUMBER>
  x_descr1
x_descr2"
                    }
                });
    }

    [Fact]
    public void Errors_when_duplicate_description_properties_are_used_in_OptionAttribute()
    {
        new AppRunner<OptionDupeDescriptions>().Verify(new Scenario
        {
            When = { Args = "" },
            Then =
            {
                ExitCode = 1,
                Output = "CommandDotNet.InvalidConfigurationException: Multiple description properties were set for " +
                         "CommandDotNet.Tests.FeatureTests.Help.ArgumentAttributesHelpTests+OptionDupeDescriptions.Do.my_option: Description, DescriptionLines. Only one can be set."
            }
        });
    }

    [Fact]
    public void Errors_when_duplicate_description_properties_are_used_in_OperandAttribute()
    {
        new AppRunner<OperandDupeDescriptions>().Verify(new Scenario
        {
            When = { Args = "" },
            Then =
            {
                ExitCode = 1,
                Output = "CommandDotNet.InvalidConfigurationException: Multiple description properties were set for " +
                         "CommandDotNet.Tests.FeatureTests.Help.ArgumentAttributesHelpTests+OperandDupeDescriptions.Do.my_operand: Description, DescriptionLines. Only one can be set."
            }
        });
    }

    [Fact]
    public void Errors_when_description_and_description_method_are_both_used_in_OptionAttribute()
    {
        new AppRunner<OptionDupeDescriptionAndMethod>().Verify(new Scenario
        {
            When = { Args = "" },
            Then =
            {
                ExitCode = 1,
                Output = "CommandDotNet.InvalidConfigurationException: Multiple description properties were set for " +
                         "CommandDotNet.Tests.FeatureTests.Help.ArgumentAttributesHelpTests+OptionDupeDescriptionAndMethod.Do.my_option: Description, DescriptionMethod. Only one can be set."
            }
        });
    }

    [Fact]
    public void Errors_when_description_lines_and_description_method_are_both_used_in_OperandAttribute()
    {
        new AppRunner<OperandDupeDescriptionLinesAndMethod>().Verify(new Scenario
        {
            When = { Args = "" },
            Then =
            {
                ExitCode = 1,
                Output = "CommandDotNet.InvalidConfigurationException: Multiple description properties were set for " +
                         "CommandDotNet.Tests.FeatureTests.Help.ArgumentAttributesHelpTests+OperandDupeDescriptionLinesAndMethod.Do.my_operand: DescriptionLines, DescriptionMethod. Only one can be set."
            }
        });
    }

    [Fact]
    public void DescriptionMethod_Works_For_Option()
    {
        new AppRunner<AppWithDescriptionMethod>(TestAppSettings.DetailedHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "DoWithOption -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll DoWithOption [options] <arg>

Arguments:

  arg  <TEXT>

Options:

  -t | --targets  <TEXT>
  The targets to sync, e.g. schemas, workflows, app, rules."
                    }
                });
    }

    [Fact]
    public void DescriptionMethod_Works_For_Operand()
    {
        new AppRunner<AppWithDescriptionMethod>(TestAppSettings.DetailedHelp)
            .Verify(
                new Scenario
                {
                    When = {Args = "DoWithOperand -h"},
                    Then =
                    {
                        Output = @"Usage: testhost.dll DoWithOperand <operand>

Arguments:

  operand  <TEXT>
  Available targets: target1, target2, target3"
                    }
                });
    }

    private class App
    {
        public void Do(
            [Option(Description = "option-descr")] string option,
            [Operand(Description = "operand-descr")] string operand) { }

        public void Multiline(
            [Option(DescriptionLines = ["x_descr1", "x_descr2"])] int option,
            [Operand(DescriptionLines = ["y_descr1", "y_descr2"])] int operand) { }
    }

    private class OptionDupeDescriptions
    {
        public void Do([Option(Description = "", DescriptionLines = [""])] string my_option) { }
    }
    
    private class OperandDupeDescriptions
    {
        public void Do([Operand(Description = "", DescriptionLines = [""])] string my_operand) { }
    }
    
    private class OptionDupeDescriptionAndMethod
    {
        public void Do([Option(Description = "some description"), DescriptionMethod(nameof(GetTargetDescription))] string my_option) { }
        
        private static string GetTargetDescription() => "Method description";
    }
    
    private class OperandDupeDescriptionLinesAndMethod
    {
        public void Do([Operand(DescriptionLines = ["line1", "line2"]), DescriptionMethod(nameof(GetTargetDescription))] string my_operand) { }
        
        private static string GetTargetDescription() => "Method description";
    }
    
    private class AppWithDescriptionMethod
    {
        public void DoWithOption(
            [Option('t', "targets"), DescriptionMethod(nameof(DescribeTargets))] string targets,
            string arg) { }
            
        public void DoWithOperand(
            [Operand, DescriptionMethod(nameof(DescribeOperandTargets))] string operand) { }
        
        private static string DescribeTargets()
        {
            return "The targets to sync, e.g. schemas, workflows, app, rules.";
        }
        
        private static string DescribeOperandTargets()
        {
            return "Available targets: target1, target2, target3";
        }
    }
}