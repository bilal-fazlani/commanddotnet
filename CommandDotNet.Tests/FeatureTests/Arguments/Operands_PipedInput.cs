using System.Collections.Generic;
using CommandDotNet.Parsing;
using CommandDotNet.Tests.Utils;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Operands_PipedInput
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Operands_PipedInput(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        [Fact]
        public void GivenPipedInputAndNoExplicitValue_PipedInputIsAppended()
        {
            var result = new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .RunInMem(
                $"{nameof(App.List)}".SplitArgs(), _testOutputHelper, 
                pipedInput: new[] {"aaa", "bbb"});

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<List<string>>().Should().BeEquivalentTo("aaa", "bbb");
        }

        [Fact]
        public void GivenPipedInputAndExplicitValue_AppendsPipedToExplicit()
        {
            var result = new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .RunInMem(
                    $"{nameof(App.List)} aaa bbb".SplitArgs(), _testOutputHelper,
                    pipedInput: new[] {"ccc", "ddd"});

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<List<string>>().Should().BeEquivalentTo("aaa", "bbb", "ccc", "ddd");
        }

        [Fact]
        public void GivenNoListArg_PipedInputIsIgnored()
        {
            var result = new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .RunInMem(
                    $"{nameof(App.Single)} single".SplitArgs(), _testOutputHelper,
                    pipedInput: new[] {"aaa"});

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<string>().Should().Be("single");
            result.TestOutputs.Get<List<string>>().Should().BeNull();
        }

        [Fact]
        public void GivenSingleAndListArg_AndNoArgValuesExplicitlyProvided_PipedInputAppendedToListArg()
        {
            var result = new AppRunner<App>()
                .AppendPipedInputToOperandList()
                .RunInMem(
                    $"{nameof(App.SingleAndList)}".SplitArgs(), _testOutputHelper,
                    pipedInput: new[] {"aaa", "bbb"});

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<string>().Should().BeNull();
            result.TestOutputs.Get<List<string>>().Should().BeEquivalentTo("aaa", "bbb");
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