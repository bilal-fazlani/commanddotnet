using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class GuaranteeOperandOrderInArgModelTests
    {
        private static readonly AppSettings OperandModeWithOutGuarantee = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Operand);
        private static readonly AppSettings OptionModeWithOutGuarantee = TestAppSettings.BasicHelp.Clone(a => a.DefaultArgumentMode = ArgumentMode.Option);
        private static readonly AppSettings OperandModeWithGuarantee = OperandModeWithOutGuarantee.Clone(a => a.GuaranteeOperandOrderInArgumentModels = true);
        private static readonly AppSettings OptionModeWithGuarantee = OptionModeWithOutGuarantee.Clone(a => a.GuaranteeOperandOrderInArgumentModels = true);

        private readonly ITestOutputHelper _output;

        public GuaranteeOperandOrderInArgModelTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GivenOptionMode_WithOutGuarantee_UnattributedArgModel_Should_BeOk()
        {
            new AppRunner<UArgModelApp>(OptionModeWithOutGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then = { ExitCode = 0 }
                });
        }

        [Fact]
        public void GivenOperandMode_WithOutGuarantee_UnattributedArgModel_Should_BeOk()
        {
            new AppRunner<UArgModelApp>(OperandModeWithOutGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then = { ExitCode = 0 }
                });
        }

        [Fact]
        public void GivenOptionMode_WithGuarantee_UnattributedArgModel_Should_BeOk()
        {
            new AppRunner<UArgModelApp>(OptionModeWithGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then = { ExitCode = 0 }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_UnattributedArgModel_Should_Detect_UnattributedOperand()
        {
            new AppRunner<UArgModelApp>(OperandModeWithGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        ExitCode = 1,
                        Result = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 "Property: CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedArgModel.Arg1"
                    }
                });
        }

        [Fact]
        public void GivenOptionMode_WithGuarantee_AttributedArgModel_Should_BeOk()
        {
            new AppRunner<AArgModelApp>(OptionModeWithGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        ExitCode = 0,
                        ResultsContainsTexts =
                        {
                            @"Arguments:
  Operand1",
                            @"Options:
  --Arg1"
                        }
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_AttributedArgModel_Should_ListOperandsInCorrectOrder()
        {
            new AppRunner<AArgModelApp>(OperandModeWithGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        ExitCode = 0,
                        ResultsContainsTexts =
                        {
                            @"Arguments:
  Arg1
  Operand1"
                        }
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_AttributedNestedModel_With_AttributedArgModel_Should_BeOk()
        {
            new AppRunner<ANestedModelAArgModelApp>(OperandModeWithGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        ExitCode = 0,
                        ResultsContainsTexts =
                        {
                            @"Arguments:
  Arg1
  Operand1
  Arg2
  Operand2"
                        }
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_UnattributedNestedModel_With_UnattributedArgModel_Should_Detect_UnattributedOperand()
        {
            new AppRunner<UNestedModelUArgModelApp>(OperandModeWithGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        ExitCode = 1,
                        Result = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 "Property: CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedArgModel.Arg1"
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_UnattributedNestedModel_With_AttributedArgModel_Should_Detect_UnattributedArgModel()
        {
            new AppRunner<UNestedModelAArgModelApp>(OperandModeWithGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        ExitCode = 1,
                        Result = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 @"Properties:
  CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedNestedModelAttributedArgModel.Model"
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_AttributedNestedModel_With_UnattributedArgModel_Should_Detect_UnattributedOperand()
        {
            new AppRunner<ANestedModelUArgModelApp>(OperandModeWithGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        ExitCode = 1,
                        Result = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 "Property: CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedArgModel.Arg1"
                    }
                });
        }

        [Fact]
        public void GivenOperandMode_WithGuarantee_DeepNestedUnattributedArgModel_Should_Detect_All_UnattributedArgModels()
        {
            new AppRunner<DeepNestedUArgModelsApp>(OperandModeWithGuarantee)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        ExitCode = 1,
                        Result = "Operand property must be attributed with OperandAttribute or " +
                                 "OrderByPositionInClassAttribute to guarantee consistent order. " +
                                 @"Properties:
  CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+UnattributedNestedModelAttributedArgModel.Model
  CommandDotNet.Tests.FeatureTests.Arguments.GuaranteeOperandOrderInArgModelTests+DeepNestedUnattributedArgModels.Model"
                    }
                });
        }

        // Invalid
        public class UArgModelApp { public void Do(UnattributedArgModel model) { } }
        // Valid & can verify order
        public class AArgModelApp { public void Do(AttributedArgModel model) { } }
        // Invalid
        public class UNestedModelUArgModelApp { public void Do(UnattributedNestedModelUnattributedArgModel model) { } }
        // Invalid
        public class UNestedModelAArgModelApp { public void Do(UnattributedNestedModelAttributedArgModel model) { } }
        // Invalid
        public class ANestedModelUArgModelApp { public void Do(AttributedNestedModelUnattributedArgModel model) { } }
        // Valid & can verify order
        public class ANestedModelAArgModelApp { public void Do(AttributedNestedModelAttributedArgModel model) { } }
        // Valid & can verify order
        public class DeepNestedUArgModelsApp { public void Do(DeepNestedUnattributedArgModels model) { } }

        // Invalid
        public class UnattributedArgModel : IArgumentModel
        {
            public string Arg1 { get; set; }
        }

        // Valid & can verify order
        public class AttributedArgModel : IArgumentModel
        {
            [OrderByPositionInClass]
            public string Arg1 { get; set; }

            [Operand]
            public string Operand1 { get; set; }
        }

        // Invalid
        public class DeepNestedUnattributedArgModels : IArgumentModel
        {
            // operand must be attributed so we can get parent arg model alerts
            public UnattributedNestedModelAttributedArgModel Model { get; set; }
        }

        // Invalid
        public class UnattributedNestedModelUnattributedArgModel : IArgumentModel
        {
            public UnattributedArgModel Model { get; set; }
        }

        // Invalid
        public class UnattributedNestedModelAttributedArgModel : IArgumentModel
        {
            public AttributedArgModel Model { get; set; }
        }

        // Invalid
        public class AttributedNestedModelUnattributedArgModel : IArgumentModel
        {
            [OrderByPositionInClass]
            public UnattributedArgModel Model { get; set; }
        }

        // Valid & can verify order
        public class AttributedNestedModelAttributedArgModel : IArgumentModel
        {
            [OrderByPositionInClass]
            public AttributedArgModel Model { get; set; }

            [OrderByPositionInClass]
            public string Arg2 { get; set; }

            [Operand]
            public string Operand2 { get; set; }
        }
    }
}