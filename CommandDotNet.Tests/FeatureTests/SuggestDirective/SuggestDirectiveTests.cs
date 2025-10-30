using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.SuggestDirective;

public class SuggestDirectiveTests
{
    public SuggestDirectiveTests(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }

    [Fact]
    public void RootLevel_ShowsCommands()
    {
        new AppRunner<App>()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest]" },
                Then =
                {
                    ExitCode = 0,
                    OutputContainsTexts = { "Add", "List", "Process" }
                }
            });
    }

    [Fact]
    public void RootLevel_ShowsOptions()
    {
        new AppRunner<App>()
            .UseDefaultMiddleware()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest]" },
                Then =
                {
                    ExitCode = 0,
                    OutputContainsTexts = { "--help" }
                }
            });
    }

    [Fact]
    public void AfterCommand_ShowsSubcommands()
    {
        new AppRunner<App>()
            .UseDefaultMiddleware()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest] Add" },
                Then =
                {
                    ExitCode = 0,
                    OutputContainsTexts = { "Item", "Range" }
                }
            });
    }

    [Fact]
    public void AfterCommand_ShowsOptions()
    {
        new AppRunner<App>()
            .UseDefaultMiddleware()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest] Process" },
                Then =
                {
                    ExitCode = 0,
                    OutputContainsTexts = { "--level" }
                }
            });
    }

    [Fact]
    public void PartialCommand_FiltersSubcommands()
    {
        new AppRunner<App>()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest] A" },
                Then =
                {
                    ExitCode = 0, // Suggestions provided successfully
                    OutputContainsTexts = { "Add" },
                    OutputNotContainsTexts = { "List", "Process" }
                }
            });
    }

    [Fact]
    public void EnumParameter_ShowsEnumValues()
    {
        new AppRunner<App>()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest] Process --level" },
                Then =
                {
                    ExitCode = 0,
                    OutputContainsTexts = { "Debug", "Info", "Warning", "Error" }
                }
            });
    }

    [Fact]
    public void EnumParameter_PartialValue_Filters()
    {
        new AppRunner<App>()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest] Process --level Wa" },
                Then =
                {
                    ExitCode = 0, // Suggestions provided successfully
                    OutputContainsTexts = { "Warning" },
                    OutputNotContainsTexts = { "Debug", "Info", "Error" }
                }
            });
    }

    [Fact]
    public void NestedSubcommands_ShowsCorrectLevel()
    {
        new AppRunner<App>()
            .UseDefaultMiddleware()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest] Add Item" },
                Then =
                {
                    ExitCode = 0,
                    OutputContainsTexts = { "--name" }
                }
            });
    }

    [Fact]
    public void DoesNotShowHiddenOptions()
    {
        new AppRunner<App>()
            .UseSuggestDirective()
            .Configure(c =>
            {
                c.BuildEvents.OnCommandCreated += args =>
                {
                    var hiddenOption = args.CommandBuilder.Command.Find<Option>("Hidden");
                    if (hiddenOption is not null)
                    {
                        hiddenOption.Hidden = true;
                    }
                };
            })
            .Verify(new Scenario
            {
                When = { Args = "[suggest] List" },
                Then =
                {
                    ExitCode = 0,
                    OutputNotContainsTexts = { "--Hidden" }
                }
            });
    }

    [Fact]
    public void ShowsHelpOption()
    {
        new AppRunner<App>()
            .UseDefaultMiddleware()
            .UseSuggestDirective()
            .Verify(new Scenario
            {
                When = { Args = "[suggest] Add" },
                Then =
                {
                    ExitCode = 0,
                    // Help option present but command has subcommands, so they're shown
                    OutputContainsTexts = { "Item", "Range" }
                }
            });
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public class App
    {
        [Subcommand]
        public class Add
        {
            [Command]
            public void Item([Option] string name = "")
            {
            }

            [Command]
            public void Range([Option] int count = 0)
            {
            }
        }

        [Command]
        public void List(ListArgs args)
        {
        }

        [Command]
        public void Process([Option] LogLevel level = LogLevel.Info)
        {
        }
    }

    public class ListArgs : IArgumentModel
    {
        [Option]
        public bool Hidden { get; set; }
    }
}
