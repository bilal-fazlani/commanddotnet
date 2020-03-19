using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Tests.ScenarioFramework;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class DefaultArgumentModeTests : TestBase
    {
        private static readonly AppSettings OperandMode = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Operand);
        private static readonly AppSettings OptionMode = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Option);

        public DefaultArgumentModeTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GivenOperandMode_InInterceptor_NonAttributedParamsDefaultTo_Operand()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = OperandMode },
                WhenArgs = "-h",
                Then =
                {
                    ResultsNotContainsTexts = { "Arguments" },
                    ResultsContainsTexts =
                    {
                        @"Options:
  --ctorDefault
  --ctorOption"
                    }
                }
            });
        }

        [Fact]
        public void GivenOperandMode_InMethod_NonAttributedParamsDefaultTo_Operand()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = OperandMode },
                WhenArgs = "Method -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  default
  operand",
                        @"Options:
  --option"
                    }
                }
            });
        }

        [Fact]
        public void GivenOperandMode_InModel_NonAttributedParamsDefaultTo_Operand()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = OperandMode },
                WhenArgs = "Model -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  Default
  Operand",
                        @"Options:
  --Option"
                    }
                }
            });
        }

        [Fact]
        public void GivenOptionMode_InInterceptor_NonAttributedParamsDefaultTo_Option()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = OptionMode },
                WhenArgs = "-h",
                Then =
                {
                    ResultsNotContainsTexts = { "Arguments" },
                    ResultsContainsTexts =
                    {
                        @"Options:
  --ctorDefault
  --ctorOption"
                    }
                }
            });
        }

        [Fact]
        public void GivenOptionMode_InMethod_NonAttributedParamsDefaultTo_Option()
        {
            Verify(new Scenario<App>
            {
                Given = { AppSettings = OptionMode },
                WhenArgs = "Method -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  operand",
                        @"Options:
  --default
  --option"
                    }
                }
            });
        }

        [Fact]
        public void GivenOptionMode_InModel_NonAttributedParamsDefaultTo_Option()
        {
            Verify(new Scenario<App>
            {
                Given = {AppSettings = OptionMode},
                WhenArgs = "Model -h",
                Then =
                {
                    ResultsContainsTexts =
                    {
                        @"Arguments:
  Operand",
                        @"Options:
  --Default
  --Option"
                    }
                }
            });
        }

        public class App
        {
            public Task<int> Middleware(CommandContext context, ExecutionDelegate next, string ctorDefault, [Option] string ctorOption)
            {
                return next(context);
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