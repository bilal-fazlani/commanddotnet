using System.Collections.Generic;
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
            var result = new AppRunner<App>().RunInMem(
                "PipeList".SplitArgs(), _testOutputHelper, 
                pipedInput: new[] {"aaa", "bbb"});

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<List<string>>().Should().BeEquivalentTo("aaa", "bbb");
        }

        [Fact]
        public void GivenNoPipedInputAndExplicitValue_ExplicitValueIsMapped()
        {
            var result = new AppRunner<App>().RunInMem(
                "PipeList aaa bbb".SplitArgs(), _testOutputHelper,
                pipedInput: null);

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<List<string>>().Should().BeEquivalentTo("aaa", "bbb");
        }

        [Fact]
        public void GivenPipedInputAndExplicitValue_AppendsPipedToExplicit()
        {
            var result = new AppRunner<App>().RunInMem(
                "PipeList aaa bbb".SplitArgs(), _testOutputHelper,
                pipedInput: new[] { "ccc", "ddd" });

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<List<string>>().Should().BeEquivalentTo("aaa", "bbb", "ccc", "ddd");
        }

        [Fact]
        public void GivenNoPipedInputAndNoExplicitValue_NoValuesMapped()
        {
            var result = new AppRunner<App>().RunInMem(
                "PipeList".SplitArgs(), _testOutputHelper,
                pipedInput: null);

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<List<string>>().Should().BeNull();
        }

        [Fact]
        public void GivenNoOperandsEnablePipedInput_InputIsNotMapped()
        {
            var result = new AppRunner<App>().RunInMem(
                "NoPiping".SplitArgs(), _testOutputHelper,
                pipedInput: null);

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<string>().Should().BeNull();
        }
        
        [Fact]
        public void GivenPipedInputAsList_AllValuesAreAppended()
        {
            var result = new AppRunner<App>().RunInMem(
                "PipeList".SplitArgs(), _testOutputHelper,
                pipedInput: new[] { "aaa", "bbb" });

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<List<string>>().Should().BeEquivalentTo("aaa", "bbb");
        }

        [Fact]
        public void GivenPipedInputAsSingle_AllValuesAreAppended()
        {
            var result = new AppRunner<App>().RunInMem(
                "PipeList".SplitArgs(), _testOutputHelper,
                pipedInput: new[] { "aaa" });

            result.ExitCode.Should().Be(0);
            result.TestOutputs.Get<List<string>>().Should().BeEquivalentTo("aaa");
        }

        [Fact]
        public void InvalidConfig_SingleValueOperandEnablePipedInput_ReturnsError()
        {
            var result = new AppRunner<App>().RunInMem(
                "InvalidPipeSingle".SplitArgs(), _testOutputHelper,
                pipedInput: new[] { "aaa", "bbb" });

            result.ExitCode.Should().Be(1);
            result.OutputShouldBe("Piped input can only be enabled for multi-value operands. `argument` is not multi-value.");
        }

        [Fact]
        public void InvalidConfig_MultipleOperandsEnablePipedInput_ReturnsError()
        {
            var result = new AppRunner<App>().RunInMem(
                "InvalidMultiArg".SplitArgs(), _testOutputHelper,
                pipedInput: new[] { "aaa", "bbb" });

            result.ExitCode.Should().Be(1);
            result.OutputShouldBe("only one operand can enable piped input. enabled operands:arg1,pipedArgs");
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void NoPiping(string argument)
            {
                TestOutputs.CaptureIfNotNull(argument);
            }

            public int InvalidPipeSingle([Operand(AppendPipedInput = true)] string argument)
            {
                return 5;
            }

            public void PipeList([Operand(AppendPipedInput = true)] List<string> arguments)
            {
                TestOutputs.CaptureIfNotNull(arguments);
            }

            public void MultiArg([Operand] string arg1, [Operand(AppendPipedInput = true)] List<string> pipedArgs)
            {
                TestOutputs.CaptureIfNotNull(arg1);
                TestOutputs.CaptureIfNotNull(pipedArgs);
            }

            public int InvalidMultiArg([Operand(AppendPipedInput = true)] string arg1, [Operand(AppendPipedInput = true)] List<string> pipedArgs)
            {
                return 5;
            }
        }
    }
}