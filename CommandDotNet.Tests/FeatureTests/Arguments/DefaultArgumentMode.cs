using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class DefaultArgumentMode : ScenarioTestBase<DefaultArgumentMode>
    {
        private static readonly AppSettings OperandMode = TestAppSettings.BasicHelp.Clone(a => a.MethodArgumentMode = ArgumentMode.Operand);
        private static readonly AppSettings OptionMode = TestAppSettings.BasicHelp.Clone(a => a.MethodArgumentMode = ArgumentMode.Option);

        public DefaultArgumentMode(ITestOutputHelper output) : base(output)
        {
        }
        
        public static Scenarios Scenarios =>
            new Scenarios
            {
                new Given<App>(
                    $"Ctor - Mode={ArgumentMode.Option} - non-attributed params default to {ArgumentMode.Option}")
                {
                    And = {AppSettings = OptionMode},
                    WhenArgs = "-h",
                    Then =
                    {
                        ResultsNotContainsTexts = { "Arguments" },
                        ResultsContainsTexts =
                        {
                            @"Options:
  --ctorDefault  
  --ctorOption
  -h | --help    Show help information"
                        }
                    }
                },
                new Given<App>(
                    $"Ctor - Mode={ArgumentMode.Operand} - non-attributed params default to {ArgumentMode.Option}")
                {
                    And = {AppSettings = OperandMode},
                    WhenArgs = "-h",
                    Then =
                    {
                        ResultsNotContainsTexts = { "Arguments" },
                        ResultsContainsTexts =
                        {
                            @"Options:
  --ctorDefault  
  --ctorOption
  -h | --help    Show help information"
                        }
                    }
                },
                new Given<App>(
                    $"Method - Mode={ArgumentMode.Option} - non-attributed params default to {ArgumentMode.Option}")
                {
                    And = {AppSettings = OptionMode},
                    WhenArgs = "Method -h",
                    Then =
                    {
                        ResultsContainsTexts =
                        {
                            @"Arguments:
  operand",
                            @"Options:
  --default
  --option
  -h | --help  Show help information"
                        }
                    }
                },
                new Given<App>(
                    $"Method - Mode={ArgumentMode.Operand} - Method - non-attributed params default to {ArgumentMode.Operand}")
                {
                    And = {AppSettings = OperandMode},
                    WhenArgs = "Method -h",
                    Then =
                    {
                        ResultsContainsTexts =
                        {
                            @"Arguments:
  default
  operand",
                            @"Options:
  --option
  -h | --help  Show help information"
                        }
                    }
                },
                new Given<App>(
                    $"Model - Mode={ArgumentMode.Option} - non-attributed properties default to {ArgumentMode.Option}")
                {
                    And = {AppSettings = OptionMode},
                    WhenArgs = "Model -h",
                    Then =
                    {
                        ResultsContainsTexts =
                        {
                            @"Arguments:
  Operand",
                            @"Options:
  --Default
  --Option
  -h | --help  Show help information"
                        }
                    }
                },
                new Given<App>(
                    $"Model - Mode={ArgumentMode.Operand} - non-attributed properties default to {ArgumentMode.Operand}")
                {
                    And = {AppSettings = OperandMode},
                    WhenArgs = "Model -h",
                    Then =
                    {
                        ResultsContainsTexts =
                        {
                            @"Arguments:
  Default
  Operand",
                            @"Options:
  --Option
  -h | --help  Show help information"
                        }
                    }
                },
            };

        public class App
        {
            public App(string ctorDefault, [Option] string ctorOption)
            {

            }

            public void Model(Model model)
            {
            }

            public void Method(string @default, [Operand] string operand, [Option] string option)
            {
            }
        }

        public class Model : IArgumentModel
        {
            public string Default { get; set; }
            [Operand]
            public string Operand { get; set; }
            [Option]
            public string Option { get; set; }
        }
    }
}