using System.Collections.Generic;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class PipedInputTargetTests
    {
        public PipedInputTargetTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void Assign_piped_input_target_as_value_when_AppSettings_DefaultPipeTargetSymbol_is_null()
        {
            new AppRunner<App>(new AppSettings{Arguments = {DefaultPipeTargetSymbol = null}})
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = "Lists -o ^",
                        PipedInput = new[] { "aaa", "bbb" }
                    },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(new[] { "^" }, null) }
                });
        }

        [Fact]
        public void Use_piped_symbol_specified_in_pipeto_directive()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = "[pipeto:$pipe$] Lists -o $pipe$",
                        PipedInput = new[] { "aaa", "bbb" }
                    },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(new[] { "aaa", "bbb" }, null) }
                });
        }

        [Fact]
        public void Assign_piped_input_to_option_list_containing_piped_symbol()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = "Lists -o ^",
                        PipedInput = new[] { "aaa", "bbb" }
                    },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(new[] { "aaa", "bbb" }, null) }
                });
        }

        [Fact]
        public void Assign_piped_input_to_operand_list_containing_piped_symbol()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = "Lists ^",
                        PipedInput = new[] { "aaa", "bbb" }
                    },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(null, new[] { "aaa", "bbb" }) }
                });
        }

        [Fact]
        public void Assign_piped_input_to_option_single_containing_piped_symbol()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = "Singles -o ^",
                        PipedInput = new[] { "aaa", "bbb" }
                    },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe("aaa", null) }
                });
        }

        [Fact]
        public void Assign_piped_input_to_operand_single_containing_piped_symbol()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = "Singles ^",
                        PipedInput = new[] { "aaa", "bbb" }
                    },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(null, "aaa") }
                });
        }

        [Fact]
        public void Error_when_piped_symbol_provided_for_more_than_one_argument()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = "Lists -o ^ ^",
                        PipedInput = new[] { "aaa", "bbb" }
                    },
                    Then =
                    {
                        ExitCode = 1,
                        Output = "Piped input can only target a single argument, but the following were targeted: operandList, optionList"
                    }
                });
        }

        private class App
        {
            public void Singles([Option('o')] string? option, [Operand] string? operand)
            {
            }

            public void Lists([Option('o')] List<string>? optionList, [Operand] List<string>? operandList)
            {
            }
        }
    }
}