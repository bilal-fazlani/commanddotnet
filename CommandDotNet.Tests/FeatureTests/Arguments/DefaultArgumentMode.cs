using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class DefaultArgumentMode : ScenarioTestBase<DefaultArgumentMode>
    {
        private static readonly AppSettings OperandMode = TestAppSettings.BasicHelp.Clone(a => a.MethodArgumentMode = ArgumentMode.Parameter);
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
                        ResultsNotContainsTexts = { "Arguments:" },
                        ResultsContainsTexts =
                        {
                            @"Options:
  -h | --help    Show help information
  --ctorDefault  
  --ctorOption"
                        }
                    }
                },
                new Given<App>(
                    $"Ctor - Mode={ArgumentMode.Parameter} - non-attributed params default to {ArgumentMode.Option}")
                {
                    And = {AppSettings = OperandMode},
                    WhenArgs = "-h",
                    Then =
                    {
                        ResultsNotContainsTexts = { "Arguments:" },
                        ResultsContainsTexts =
                        {
                            @"Options:
  -h | --help    Show help information
  --ctorDefault  
  --ctorOption"
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
  -h | --help  Show help information
  --default
  --option"
                        }
                    }
                },
                new Given<App>(
                    $"Method - Mode={ArgumentMode.Parameter} - Method - non-attributed params default to {ArgumentMode.Parameter}")
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
  -h | --help  Show help information
  --option"
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
  -h | --help  Show help information
  --Default
  --Option"
                        }
                    }
                },
                new Given<App>(
                    $"Model - Mode={ArgumentMode.Parameter} - non-attributed properties default to {ArgumentMode.Parameter}")
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
  -h | --help  Show help information
  --Option"
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

            public void Method(string @default, [Argument] string operand, [Option] string option)
            {
            }
        }

        public class Model : IArgumentModel
        {
            public string Default { get; set; }
            [Argument]
            public string Operand { get; set; }
            [Option]
            public string Option { get; set; }
        }
    }
}