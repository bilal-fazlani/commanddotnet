using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class DefaultArgumentMode : TestBase
    {
        private static readonly AppSettings OperandMode = TestAppSettings.BasicHelp.Clone(a => a.MethodArgumentMode = ArgumentMode.Operand);
        private static readonly AppSettings OptionMode = TestAppSettings.BasicHelp.Clone(a => a.MethodArgumentMode = ArgumentMode.Option);
        private static readonly AppSettings DeprecatedParameterMode = TestAppSettings.BasicHelp.Clone(a => a.MethodArgumentMode = ArgumentMode.Parameter);

        public DefaultArgumentMode(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GivenOperandMode_InCtor_NonAttributedParamsDefaultTo_Operand()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = OperandMode },
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
            });
        }

        [Fact]
        public void GivenOperandMode_InMethod_NonAttributedParamsDefaultTo_Operand()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = OperandMode },
                WhenArgs = "Method -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  default
  operand
  argument",
                        @"Options:
  --option
  -h | --help  Show help information"
                    }
                }
            });
        }

        [Fact]
        public void GivenOperandMode_InModel_NonAttributedParamsDefaultTo_Operand()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = OperandMode },
                WhenArgs = "Model -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  Default
  Operand
  Argument",
                        @"Options:
  --Option
  -h | --help  Show help information"
                    }
                }
            });
        }

        [Fact]
        public void GivenOptionMode_InCtor_NonAttributedParamsDefaultTo_Option()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = OptionMode },
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
            });
        }

        [Fact]
        public void GivenOptionMode_InMethod_NonAttributedParamsDefaultTo_Option()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = OptionMode },
                WhenArgs = "Method -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  operand
  argument",
                        @"Options:
  --default
  --option
  -h | --help  Show help information"
                    }
                }
            });
        }

        [Fact]
        public void GivenOptionMode_InModel_NonAttributedParamsDefaultTo_Option()
        {
            Verify(new Given<App>
            {
                And = {AppSettings = OptionMode},
                WhenArgs = "Model -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  Operand
  Argument",
                        @"Options:
  --Default
  --Option
  -h | --help  Show help information"
                    }
                }
            });
        }

        [Fact]
        public void GivenObsoleteParameterMode_InCtor_NonAttributedParamsDefaultTo_Operand()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DeprecatedParameterMode },
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
            });
        }

        [Fact]
        public void GivenObsoleteParameterMode_InMethod_NonAttributedParamsDefaultTo_Operand()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DeprecatedParameterMode },
                WhenArgs = "Method -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  default
  operand
  argument",
                        @"Options:
  --option
  -h | --help  Show help information"
                    }
                }
            });
        }

        [Fact]
        public void GivenObsoleteParameterMode_InModel_NonAttributedParamsDefaultTo_Operand()
        {
            Verify(new Given<App>
            {
                And = { AppSettings = DeprecatedParameterMode },
                WhenArgs = "Model -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  Default
  Operand
  Argument",
                        @"Options:
  --Option
  -h | --help  Show help information"
                    }
                }
            });
        }

        public class App
        {
            public App(string ctorDefault, [Option] string ctorOption)
            {

            }

            public void Model(Model model)
            {
            }

            public void Method(string @default, [Operand] string operand, [Option] string option, [Argument] string argument)
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
            [Argument]
            public string Argument { get; set; }
        }
    }
}