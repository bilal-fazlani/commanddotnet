using System.Collections.Generic;
using CommandDotNet.Tests.Utils;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class PromptForMissingOperands
    {
        private static readonly AppSettings PromptingEnabled = TestAppSettings.TestDefault.Clone(s => s.PromptForMissingOperands = true);
        private static readonly AppSettings PromptingDisabled = TestAppSettings.TestDefault.Clone(s => s.PromptForMissingOperands = false);

        private readonly ITestOutputHelper _output;

        public PromptForMissingOperands(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void PromptingEnabled_WhenOperandProvidedButMissingOption_DoesNotPrompt()
        {
            var result = new AppRunner<App>(PromptingEnabled)
                .RunInMem("Do something".SplitArgs(), _output, onReadLine: console => "yes");

            var doResult = result.TestOutputs.Get<App.DoResult>();
            doResult.opt1.Should().BeNull();
            doResult.arg1.Should().Be("something");
        }

        [Fact]
        public void PromptingEnabled_WhenOperandAndOptionProvided_DoesNotPrompt()
        {
            var result = new AppRunner<App>(PromptingEnabled)
                .RunInMem("Do something --opt1 simple".SplitArgs(), _output, onReadLine: console => "yes");

            var doResult = result.TestOutputs.Get<App.DoResult>();
            doResult.opt1.Should().Be("simple");
            doResult.arg1.Should().Be("something");
        }

        [Fact]
        public void PromptingEnabled_WhenMissingOperand_DoesPrompt()
        {
            var result = new AppRunner<App>(PromptingEnabled)
                .RunInMem("Do".SplitArgs(), _output, onReadLine: console => "yes");

            var doResult = result.TestOutputs.Get<App.DoResult>();
            doResult.opt1.Should().BeNull();
            doResult.arg1.Should().Be("yes");
        }

        [Fact]
        public void PromptingDisabled_WhenMissingOperand_DoesNotPrompt()
        {
            var result = new AppRunner<App>(PromptingDisabled)
                .RunInMem("Do".SplitArgs(), _output, onReadLine: console => "yes");

            var doResult = result.TestOutputs.Get<App.DoResult>();
            doResult.opt1.Should().BeNull();
            doResult.arg1.Should().BeNull();
        }

        [Fact]
        public void PromptingEnabled_WhenOperandListProvided_DoesNotPrompt()
        {
            var result = new AppRunner<App>(PromptingEnabled)
                .RunInMem("DoList something simple".SplitArgs(), _output, onReadLine: console => "yes");

            var doResult = result.TestOutputs.Get<List<string>>();
            doResult.Count.Should().Be(2);
            doResult.Should().BeEquivalentTo("something", "simple");
        }

        [Fact]
        public void PromptingEnabled_WhenMissingOperandList_DoesPrompt()
        {
            var result = new AppRunner<App>(PromptingEnabled)
                .RunInMem("DoList".SplitArgs(), _output, onReadLine: console => "something simple");

            var doResult = result.TestOutputs.Get<List<string>>();
            doResult.Count.Should().Be(2);
            doResult.Should().BeEquivalentTo("something", "simple");
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public int Do([Option] string opt1, string arg1)
            {
                TestOutputs.Capture(new DoResult{opt1 = opt1, arg1 = arg1});
                return opt1 == arg1 ? 0 : 1;
            }

            public int DoList(List<string> args)
            {
                TestOutputs.Capture(args);
                return 0;
            }

            public class DoResult
            {
                public string opt1; 
                public string arg1;
            }
        }
    }
}