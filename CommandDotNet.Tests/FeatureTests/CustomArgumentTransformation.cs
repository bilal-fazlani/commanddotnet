using System.Linq;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Parsing;
using CommandDotNet.Tests.Utils;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CustomArgumentTransformation
    {
        private readonly AppSettings DirectivesEnabled = TestAppSettings.TestDefault.Clone(s => s.EnableDirectives = true);

        private readonly ITestOutputHelper _output;

        public CustomArgumentTransformation(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ParseDirective_OutputsResults()
        {
            var result = new AppRunner<App>(DirectivesEnabled)
                .UseArgumentTransform("test", 1,
                    tokens => new Tokens(tokens.Select(t =>
                        t.TokenType == TokenType.Value && t.Value != "Do"
                            ? Tokenizer.TokenizeValue("roses")
                            : t)))
                .RunInMem("[parse] Do --opt1 smells like".SplitArgs(), _output);

            result.OutputShouldBe(@">>> from shell
  Value    : Do
  Option   : --opt1
  Value    : smells
  Value    : like
>>> transformed after: test
  Value    : Do
  Option   : --opt1
  Value    : roses
  Value    : roses
>>> no changes after: Expand clubbed flags");
        }

        [Fact]
        public void CanRegisterCustomArgumentTransformation()
        {
            var result = new AppRunner<App>()
                .UseArgumentTransform("test", 1, 
                    tokens => new Tokens(tokens.Select(t =>
                        t.TokenType == TokenType.Value && t.Value != "Do"
                            ? Tokenizer.TokenizeValue("roses")
                            : t)))
                .RunInMem("Do --opt1 smells like".SplitArgs(), _output);

            result.TestOutputs.Get<string>().Should().Be("roses");
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public int Do([Option] string opt1, string arg1)
            {
                TestOutputs.Capture(opt1);
                return opt1 == arg1 ? 0 : 1;
            }
        }
    }
}