using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class DefaultArgumentModeTests
    {
        private static readonly AppSettings OperandMode = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Operand);
        private static readonly AppSettings OptionMode = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Option);
        private static readonly AppSettings DeprecatedParameterMode = TestAppSettings.BasicHelp.Clone(a => a.MethodArgumentMode = ArgumentMode.Parameter);

        public DefaultArgumentModeTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void GivenOperandMode_InInterceptor_NonAttributedParamsDefaultTo_Operand()
        {
            new AppRunner<App>(OperandMode).Verify(new Scenario
            {
                WhenArgs = "-h",
                Then =
                {
                    OutputNotContainsTexts = { "Arguments" },
                    OutputContainsTexts =
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
            new AppRunner<App>(OperandMode).Verify(new Scenario
            {
                WhenArgs = "Method -h",
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  default
  operand
  argument",
                        @"Options:
  --option"
                    }
                }
            });
        }

        [Fact]
        public void GivenOperandMode_InModel_NonAttributedParamsDefaultTo_Operand()
        {
            new AppRunner<App>(OperandMode).Verify(new Scenario
            {
                WhenArgs = "Model -h",
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  Default
  Operand
  Argument",
                        @"Options:
  --Option"
                    }
                }
            });
        }

        [Fact]
        public void GivenOptionMode_InInterceptor_NonAttributedParamsDefaultTo_Option()
        {
            new AppRunner<App>(OptionMode).Verify(new Scenario
            {
                WhenArgs = "-h",
                Then =
                {
                    OutputNotContainsTexts = { "Arguments" },
                    OutputContainsTexts =
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
            new AppRunner<App>(OptionMode).Verify(new Scenario
            {
                WhenArgs = "Method -h",
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  operand
  argument",
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
            new AppRunner<App>(OptionMode).Verify(new Scenario
            {
                WhenArgs = "Model -h",
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  Operand
  Argument",
                        @"Options:
  --Default
  --Option"
                    }
                }
            });
        }

        [Fact]
        public void GivenObsoleteParameterMode_InInterceptor_NonAttributedParamsDefaultTo_Operand()
        {
            new AppRunner<App>(DeprecatedParameterMode).Verify(new Scenario
            {
                WhenArgs = "-h",
                Then =
                {
                    OutputNotContainsTexts = { "Arguments" },
                    OutputContainsTexts =
                    {
                        @"Options:
  --ctorDefault
  --ctorOption"
                    }
                }
            });
        }

        [Fact]
        public void GivenObsoleteParameterMode_InMethod_NonAttributedParamsDefaultTo_Operand()
        {
            new AppRunner<App>(DeprecatedParameterMode).Verify(new Scenario
            {
                WhenArgs = "Method -h",
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  default
  operand
  argument",
                        @"Options:
  --option"
                    }
                }
            });
        }

        [Fact]
        public void GivenObsoleteParameterMode_InModel_NonAttributedParamsDefaultTo_Operand()
        {
            new AppRunner<App>(DeprecatedParameterMode).Verify(new Scenario
            {
                WhenArgs = "Model -h",
                Then =
                {
                    OutputContainsTexts =
                    {
                        @"Arguments:
  Default
  Operand
  Argument",
                        @"Options:
  --Option"
                    }
                }
            });
        }

        private class App
        {
            public Task<int> Middleware(CommandContext context, ExecutionDelegate next, string ctorDefault, [Option] string ctorOption)
            {
                return next(context);
            }

            public void Model(Model model)
            {
            }

            public void Method(string @default, [Operand] string operand, [Option] string option, [Argument] string argument)
            {
            }
        }

        private class Model : IArgumentModel
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