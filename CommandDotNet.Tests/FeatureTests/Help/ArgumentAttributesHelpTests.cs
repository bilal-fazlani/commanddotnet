using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Help
{
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
                    Output = "CommandDotNet.InvalidConfigurationException: Both Description and DescriptionLines were set for " +
                             "CommandDotNet.Tests.FeatureTests.Help.ArgumentAttributesHelpTests+OptionDupeDescriptions.Do.my_option. Only one can be set."
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
                    Output = "CommandDotNet.InvalidConfigurationException: Both Description and DescriptionLines were set for " +
                             "CommandDotNet.Tests.FeatureTests.Help.ArgumentAttributesHelpTests+OperandDupeDescriptions.Do.my_operand. Only one can be set."
                }
            });
        }

        private class App
        {
            public void Do(
                [Option(Description = "option-descr")] string option,
                [Operand(Description = "operand-descr")] string operand) { }

            public void Multiline(
                [Option(DescriptionLines = new[] { "x_descr1", "x_descr2" })] int option,
                [Operand(DescriptionLines = new[] { "y_descr1", "y_descr2" })] int operand) { }
        }

        private class OptionDupeDescriptions
        {
            public void Do([Option(Description = "", DescriptionLines = new[] { "" })] string my_option) { }
        }
        private class OperandDupeDescriptions
        {
            public void Do([Operand(Description = "", DescriptionLines = new[] { "" })] string my_operand) { }
        }
    }
}
