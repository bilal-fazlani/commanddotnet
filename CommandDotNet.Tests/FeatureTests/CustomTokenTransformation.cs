using System.Linq;
using CommandDotNet.Tests.Utils;
using CommandDotNet.Tokens;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CustomTokenTransformation
    {
        private readonly AppSettings DirectivesEnabled = TestAppSettings.TestDefault.Clone(s => s.EnableDirectives = true);

        private readonly ITestOutputHelper _output;

        public CustomTokenTransformation(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ParseDirective_OutputsResults()
        {
            var result = new AppRunner<App>(DirectivesEnabled)
                .Configure(c =>
                    c.UseTokenTransformation("test", 1,
                        tokens => new TokenCollection(tokens.Select(t =>
                            t.TokenType == TokenType.Value && t.Value != "Do"
                                ? Tokenizer.TokenizeValue("roses")
                                : t))))
                .RunInMem("[parse] Do --opt1 smells like".SplitArgs(), _output);

            result.OutputShouldBe(@"use [parse:verbose] to see results after each transformation
>>> from shell
  Directive: [parse]
  Value    : Do
  Option   : --opt1
  Value    : smells
  Value    : like
>>> transformed after: test > expand-clubbed-flags > split-option-assignments
  Directive: [parse]
  Value    : Do
  Option   : --opt1
  Value    : roses
  Value    : roses");
        }

        [Fact]
        public void CanRegisterCustomTokenTransformation()
        {
            var result = new AppRunner<App>()
                .Configure(c =>
                    c.UseTokenTransformation("test", 1,
                        tokens => new TokenCollection(tokens.Select(t =>
                            t.TokenType == TokenType.Value && t.Value != "Do"
                                ? Tokenizer.TokenizeValue("roses")
                                : t))))
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