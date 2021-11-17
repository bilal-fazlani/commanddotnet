using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Option_Split_Tests
    {
        public Option_Split_Tests(ITestOutputHelper testOutputHelper)
        {
            Ambient.Output = testOutputHelper;
        }

        [Fact]
        public void Works()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "Colon --list one:two:three" },
                    Then = { AssertContext = ctx =>
                        {
                            ctx.ParamValuesShouldBe(new []{ "one", "two", "three" }.AsEnumerable());
                        }
                    }
                });
        }

        [Fact]
        public void Works2()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = { Args = "Pipe --list one|two|three" },
                    Then = { AssertContext = ctx =>
                        {
                            ctx.ParamValuesShouldBe(new []{ "one", "two", "three" }.AsEnumerable());
                        }
                    }
                });
        }

        [Fact]
        public void Invalid()
        {
            new AppRunner<InvalidApp>()
                .Verify(new Scenario
                {
                    When = { Args = "NonEnum --value one:two:three" },
                    Then = { 
                        ExitCode = 1,
                        Output = "Split can only be specified for IEnumerable<T> types. " +
                                 @"CommandDotNet.Tests.FeatureTests.Arguments.Option_Split_Tests+InvalidApp.NonEnum.value is type System.String
"
                    }
                });
        }

        private class App
        {
            public void Colon([Option(Split = ':')] IEnumerable<string> list)
            {
            }
            public void Pipe([Option(Split = '|')] IEnumerable<string> list)
            {
            }
        }

        private class InvalidApp
        {
            public void NonEnum([Option(Split = ':')] string value)
            {
            }
        }
    }
}