using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.EnabledMiddlewareScenarios
{
    public class NameTransformationTests
    {
        private readonly ITestOutputHelper _output;

        public NameTransformationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TransformNameForAllTypesShouldWork()
        {
            new AppRunner<App>()
                .Configure(b => b.NameTransformation = (attributes, memberName, nameOverride, commandNodeType)
                    => $"prefix-{memberName}")
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "prefix-Do -h",
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            "prefix-Do", "prefix-option1", "prefix-operand1"
                        }
                    }
                });
        }

        [Fact]
        public void TransformNameForOnlyCommandTypesShouldWork()
        {
            new AppRunner<App>()
                .Configure(b => b.NameTransformation = (attributes, memberName, nameOverride, commandNodeType)
                    => commandNodeType.IsCommand ? $"prefix-{memberName}" : memberName)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "prefix-Do -h",
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            "prefix-Do", "option1", "operand1"
                        },
                        OutputNotContainsTexts =
                        {
                            "prefix-option1", "prefix-operand1"
                        }
                    }
                });
        }

        [Fact]
        public void TransformNameForOnlyOperandTypesShouldWork()
        {
            new AppRunner<App>()
                .Configure(b => b.NameTransformation = (attributes, memberName, nameOverride, commandNodeType)
                    => commandNodeType.IsOperand ? $"prefix-{memberName}" : memberName)
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        OutputContainsTexts =
                        {
                            "Do", "option1", "prefix-operand1"
                        },
                        OutputNotContainsTexts =
                        {
                            "prefix-option1"
                        }
                    }
                });
        }

        class App
        {
            public void Do(
                [Option] string option1,
                [Operand] string operand1)
            {

            }
        }
    }
}