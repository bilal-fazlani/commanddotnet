using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_PipedInput_Tests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Operands_PipedInput_Tests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        [Fact]
        public void GivenPipedInputAndNoExplicitValue_PipedInputIsAppended()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        Given = {PipedInput = new[] {"aaa", "bbb"}},
                        WhenArgs = $"{nameof(App.List)}",
                        Then =
                        {
                            Outputs = {new List<string> {"aaa", "bbb"}}
                        }
                    });
        }

        [Fact]
        public void GivenPipedInputAndExplicitValue_AppendsPipedToExplicit()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        Given = {PipedInput = new[] {"ccc", "ddd"}},
                        WhenArgs = $"{nameof(App.List)} aaa bbb",
                        Then =
                        {
                            Outputs = {new List<string> {"aaa", "bbb", "ccc", "ddd"}}
                        }
                    });
        }

        [Fact]
        public void GivenNoListArg_PipedInputIsIgnored()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        Given = {PipedInput = new[] {"aaa"}},
                        WhenArgs = $"{nameof(App.Single)} single",
                        Then =
                        {
                            Outputs = {"single"}
                        }
                    });
        }

        [Fact]
        public void GivenSingleAndListArg_AndNoArgValuesExplicitlyProvided_PipedInputAppendedToListArg()
        {
            new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        Given = {PipedInput = new[] {"aaa", "bbb"}},
                        WhenArgs = $"{nameof(App.SingleAndList)} single",
                        Then =
                        {
                            Outputs =
                            {
                                "single",
                                new List<string> {"aaa", "bbb"}
                            }
                        }
                    });
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Single([Operand] string singleArg)
            {
                TestOutputs.CaptureIfNotNull(singleArg);
            }

            public void List([Operand] List<string> listArgs)
            {
                TestOutputs.CaptureIfNotNull(listArgs);
            }

            public void SingleAndList([Operand] string singleArg, [Operand] List<string> listArgs)
            {
                TestOutputs.CaptureIfNotNull(singleArg);
                TestOutputs.CaptureIfNotNull(listArgs);
            }
        }
    }
}