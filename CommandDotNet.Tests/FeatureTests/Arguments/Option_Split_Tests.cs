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
        public void Split_can_be_set_on_OptionAttribute()
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
        public void Split_can_default_from_AppSettings()
        {
            new AppRunner<App>(new AppSettings { Arguments = { DefaultOptionSplit = ':' } })
                .Verify(new Scenario
                {
                    When = { Args = "Default --list one:two:three" },
                    Then = { AssertContext = ctx =>
                        {
                            ctx.ParamValuesShouldBe(new []{ "one", "two", "three" }.AsEnumerable());
                        }
                    }
                });
        }


        [Fact]
        public void Split_can_be_overridden_with_directive()
        {
            new AppRunner<App>(new AppSettings { Arguments = { DefaultOptionSplit = ':' } })
                .Verify(new Scenario
                {
                    When = { Args = "[split:,] Pipe --list one,two,three" },
                    Then = { AssertContext = ctx =>
                        {
                            ctx.ParamValuesShouldBe(new []{ "one", "two", "three" }.AsEnumerable());
                        }
                    }
                });
        }

        [Fact]
        public void Split_on_OptionAttribute_overrides_default_from_AppSettings()
        {
            new AppRunner<App>(new AppSettings{Arguments = {DefaultOptionSplit = ':'}})
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
        public void Split_can_only_be_set_for_nonstring_enumerables()
        {
            new AppRunner<InvalidApp>()
                .Verify(new Scenario
                {
                    When = { Args = "NonEnum --value one:two:three" },
                    Then = { 
                        ExitCode = 1,
                        Output = "CommandDotNet.InvalidConfigurationException: Split can only be specified for IEnumerable<T> types. " +
                                 @"CommandDotNet.Tests.FeatureTests.Arguments.Option_Split_Tests+InvalidApp.NonEnum.value is type System.String"
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

            public void Default([Option] IEnumerable<string> list)
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