using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseDirective_Transforms_Tests
    {
        public ParseDirective_Transforms_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void When_ParseT_ShowsWhenTransformDoesNotMakeChanges()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When = {Args = "[parse:t] Do"},
                    Then =
                    {
                        OutputContainsTexts = { @"token transformations:

>>> from shell
  Directive: [parse:t]
  Argument : Do" }
                    }
                });
        }

        [Fact]
        public void When_ParseT_ShowsResultsOfEveryTransform()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When = {Args = "[parse:t] Do -abc --one two --three:four --five=six seven"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { @"token transformations:

>>> from shell
  Directive: [parse:t]
  Argument : Do
  Argument : -abc
  Argument : --one
  Argument : two
  Argument : --three:four
  Argument : --five=six
  Argument : seven" }
                    }
                });
        }

        private class App
        {
            public void Do() { }
        }
    }
}
