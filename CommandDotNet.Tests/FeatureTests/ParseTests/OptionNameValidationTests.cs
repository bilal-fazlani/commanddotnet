using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseTests;

public class OptionNameValidationTests
{
    public OptionNameValidationTests(ITestOutputHelper output) => Ambient.Output = output;

    [Fact]
    public void ValidOptionName_WithAlphanumeric_IsParsedAsOption() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --opt1 value1 arg1" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe("value1", "arg1")
            }
        });

    [Fact]
    public void ValidOptionName_WithUnderscore_ThrowsUnrecognizedOptionError() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --opt_1 value1 arg1" },
            Then =
            {
                ExitCode = 1,
                OutputContainsTexts = {"Unrecognized option '--opt_1'"}
            }
        });

    [Fact]
    public void ValidOptionName_WithHyphen_ThrowsUnrecognizedOptionError() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --opt-1 value1 arg1" },
            Then =
            {
                ExitCode = 1,
                OutputContainsTexts = {"Unrecognized option '--opt-1'"}
            }
        });

    [Fact]
    public void InvalidOptionName_WithSpecialCharacters_IsTreatedAsOperand() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --opt@name" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(null, "--opt@name")
            }
        });

    [Fact]
    public void InvalidOptionName_WithDot_IsTreatedAsOperand() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --opt.name" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(null, "--opt.name")
            }
        });

    [Fact]
    public void InvalidOptionName_WithDollarSign_IsTreatedAsOperand() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --$option" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(null, "--$option")
            }
        });

    [Fact]
    public void ValidOptionName_WithValueAssignment_IsParsedCorrectly() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --opt1=value1 arg1" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe("value1", "arg1")
            }
        });

    [Fact]
    public void ValidOptionName_WithColonValueAssignment_IsParsedCorrectly() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --opt1:value1 arg1" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe("value1", "arg1")
            }
        });

    [Fact]
    public void InvalidOptionName_BeforeEqualsSeparator_IsTreatedAsOperand() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --opt@1=value1" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(null, "--opt@1=value1")
            }
        });

    [Fact]
    public void InvalidOptionName_WithParenthesis_IsTreatedAsOperand() =>
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do --opt(1)" },
            Then =
            {
                AssertContext = ctx => ctx.ParamValuesShouldBe(null, "--opt(1)")
            }
        });

    private class App
    {
        public void Do([Option] string? opt1, [Operand] string? arg1)
        {
        }
    }
}
