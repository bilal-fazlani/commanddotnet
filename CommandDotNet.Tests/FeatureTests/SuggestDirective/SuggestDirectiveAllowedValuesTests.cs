using System.Collections.Generic;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.SuggestDirective;

public class SuggestDirectiveAllowedValuesTests
{
    public SuggestDirectiveAllowedValuesTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
    public void OptionAllowedValues_AreSuggested()
    {
        new AppRunner<App>()
            .UseSuggestDirective()
            .Configure(c =>
            {
                c.BuildEvents.OnCommandCreated += args =>
                {
                    var colorOption = args.CommandBuilder.Command.Find<Option>("Color");
                    if (colorOption is not null)
                    {
                        colorOption.AllowedValues = new[] { "red", "green", "blue" };
                    }
                };
            })
            .Verify(new Scenario
            {
                When = { Args = "[suggest] Paint --Color" },
                Then =
                {
                    ExitCode = 0,
                    OutputContainsTexts = { "red", "green", "blue" }
                }
            });
    }

    [Fact]
    public void OperandAllowedValues_AreSuggested()
    {
        new AppRunner<App>()
            .UseSuggestDirective()
            .Configure(c =>
            {
                c.BuildEvents.OnCommandCreated += args =>
                {
                    var modeOperand = args.CommandBuilder.Command.Find<Operand>("Mode");
                    if (modeOperand is not null)
                    {
                        modeOperand.AllowedValues = new[] { "fast", "normal", "slow" };
                    }
                };
            })
            .Verify(new Scenario
            {
                When = { Args = "[suggest] SelectMode" },
                Then =
                {
                    ExitCode = 0,
                    OutputContainsTexts = { "fast", "normal", "slow" }
                }
            });
    }

    [Fact]
    public void AllowedValues_PartialMatch_Filtered()
    {
        new AppRunner<App>()
            .UseSuggestDirective()
            .Configure(c =>
            {
                c.BuildEvents.OnCommandCreated += args =>
                {
                    var colorOption = args.CommandBuilder.Command.Find<Option>("Color");
                    if (colorOption is not null)
                    {
                        colorOption.AllowedValues = new[] { "red", "green", "blue" };
                    }
                };
            })
            .Verify(new Scenario
            {
                When = { Args = "[suggest] Paint --Color gr" },
                Then =
                {
                    ExitCode = 0, // Suggestions provided successfully  
                    OutputContainsTexts = { "green" },
                    OutputNotContainsTexts = { "red", "blue" }
                }
            });
    }

    [Fact]
    public void NoAllowedValues_NoSuggestions()
    {
        new AppRunner<App>()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest] Echo --message" },
                Then =
                {
                    ExitCode = 0,
                    Output = "" // No suggestions for free-form text
                }
            });
    }

    public class App
    {
        [Command]
        public void Paint(PaintArgs args)
        {
        }

        [Command]
        public void SelectMode(SelectModeArgs args)
        {
        }

        [Command]
        public void Echo([Option] string message = "")
        {
        }
    }

    public class PaintArgs : IArgumentModel
    {
        [Option]
        public string? Color { get; set; }
    }

    public class SelectModeArgs : IArgumentModel
    {
        [Operand]
        public string Mode { get; set; } = "normal";
    }
}
