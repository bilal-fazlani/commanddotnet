using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using CommandDotNet.Tokens;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CustomTokenTransformationTests
    {
        private readonly ITestOutputHelper _output;

        public CustomTokenTransformationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ParseDirective_OutputsResults()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .Configure(c =>
                    c.UseTokenTransformation("test", 1,
                        (ctx, tokens) => tokens.Transform(
                            skipDirectives: true, 
                            skipSeparated: true,
                            transformation: t =>
                                t.TokenType == TokenType.Value && t.Value == "like"
                                    ? Tokenizer.TokenizeValue("roses").ToEnumerable()
                                    : t.ToEnumerable())))
                .VerifyScenario(_output, new Scenario
                {
                    WhenArgs = "[parse:tokens] Do --opt1 smells like",
                    Then =
                    {
                        Result = @"use [parse:help] to see additional parse options
>>> from shell
  Directive: [parse:tokens]
  Value    : Do
  Option   : --opt1
  Value    : smells
  Value    : like
>>> transformed after: test > expand-clubbed-flags > split-option-assignments
  Directive: [parse:tokens]
  Value    : Do
  Option   : --opt1
  Value    : smells
  Value    : roses"
                    }
                });
        }

        [Fact]
        public void CanRegisterCustomTokenTransformation()
        {
            var result = new AppRunner<App>()
                .Configure(c =>
                    c.UseTokenTransformation("test", 1,
                        (ctx, tokens) => tokens.Transform(
                            skipDirectives: true,
                            skipSeparated: true,
                            transformation: t =>
                                t.TokenType == TokenType.Value && t.Value == "roses"
                                    ? Tokenizer.TokenizeValue("roses").ToEnumerable()
                                    : t.ToEnumerable())))
                .RunInMem("Do --opt1 smells like".SplitArgs(), _output);

            result.TestOutputs.Get<string>().Should().Be("roses");
        }

        public class App
        {
            private TestOutputs TestOutputs { get; set; }

            public int Do([Option] string opt1, string arg1 = "wet dog")
            {
                TestOutputs.Capture(opt1);
                return opt1 == arg1 ? 0 : 1;
            }
        }
    }
}