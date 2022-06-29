using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Spectre;
using CommandDotNet.Spectre.Testing;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.Spectre
{
    public class SpectreArgumentPrompterTests
    {
        // Adapted from PromptForMissingArgumentTests

        public SpectreArgumentPrompterTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void WhenOperandAndOptionProvided_DoesNotPrompt()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextWithEnter("lala");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)} something --opt1 simple",
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("simple", "something"),
                        Output = ""
                    }
                });
        }

        [Fact]
        public void WhenOptionMissing_PromptsOnlyForOption()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextWithEnter("simple");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)} something"
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("simple", "something"),
                        Output = @"opt1 (Text) simple
"
                    }
                });
        }

        [Fact]
        public void WhenOperandMissing_PromptsOnlyForOperand()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextWithEnter("something");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)} --opt1 simple",
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("simple", "something"),
                        Output = @"arg1 (Text) something
"
                    }
                });
        }

        [Fact]
        public void WhenOptionAndOperandMissing_PromptsForBoth()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextsWithEnter("something", "simple");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}"
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("simple", "something"),
                        Output = @"arg1 (Text) something
opt1 (Text) simple
"
                    }
                });
        }

        [Fact]
        public void WhenOperandListProvided_DoesNotPrompt()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextWithEnter("lala");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.DoList)} something simple"
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new List<string>{"something", "simple"}),
                        Output = ""
                    }
                });
        }

        [Fact]
        public void WhenOperandListMissing_Prompts()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextsWithEnter("something", "simple", "");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.DoList)}"
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new List<string>{"something", "simple"}),
                        Output = @"args (Text)
> something
> simple
> 
"
                    }
                });
        }

        [Fact]
        public void ListPrompt_CanIncludeQuotes()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextsWithEnter("something", "simple", "'or not'", "\"so simple\"", "");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.DoList)}"
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(
                            new List<string>{"something", "simple", "'or not'", "\"so simple\""}),
                    }
                });
        }

        [Fact]
        public void WhenInterceptorOptionMissing_Prompts()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextWithEnter("1");

            new AppRunner<HierApp>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(HierApp.Do)} --inherited1 2",
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe<HierApp>(1, 2)
                    }
                });
        }

        [Fact]
        public void WhenInheritedOptionMissing_Prompts()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextWithEnter("2");

            new AppRunner<HierApp>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $" --intercept1 1 {nameof(HierApp.Do)}"
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe<HierApp>(1, 2)
                    }
                });
        }

        [Fact]
        public void WhenPasswordMissing_PromptMasksInput()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextsWithEnter("lala", "fishies");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Secure)}"
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("lala", new Password("fishies")),
                        Output = @"user (Text) lala
password (Text) *******
"
                    }
                });
        }

        [Fact]
        public void WhenFlagsMissing_DoesNotPrompt()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextsWithEnter("y", "y");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Flags)}"
                    },
                    Then = { AssertContext = ctx => ctx.ParamValuesShouldBe(null, null)}
                });
        }

        [Fact]
        public void WhenExplicitBoolOptionMissing_Prompts()
        {
            var testConsole = new AnsiTestConsole().Interactive();
            testConsole.Input.PushTextWithEnter("y"); 
            testConsole.Input.PushTextWithEnter("y");

            new AppRunner<App>(new AppSettings {Arguments = {BooleanMode = BooleanMode.Explicit }})
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Flags)}"
                    },
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(true, true)}
                });
        }

        [Fact]
        public void WhenBoolOperandMissing_Prompts()
        {
            var testConsole = new AnsiTestConsole().Interactive();
            testConsole.Input.PushTextWithEnter("y");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Bool)}"
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(true),
                        Output = @"operand1 (Boolean) [y/n] (y): y
"
                    }
                });
        }

        [Fact]
        public void CanOverridePromptText()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextWithEnter("fishies");
            testConsole.Input.PushTextWithEnter("fishies");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter(getPromptTextCallback: (ctx, arg) => "lala")
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("fishies", "fishies"),
                        Output = @"lala (Text) fishies
lala (Text) fishies
"
                    }
                });
        }

        [Fact]
        public void CanFilterListOfArgumentsForPrompting()
        {
            var testConsole = new AnsiTestConsole();
            testConsole.Input.PushTextWithEnter("something");

            new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .UseSpectreArgumentPrompter(argumentFilter: arg => arg.Name == "arg1")
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}"
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(null, "something"),
                        ExitCode = 2,
                        Output = @"arg1 (Text) something
opt1 is required"
                    }
                });
        }

        [Fact]
        public void PipedInputIsEvaluatedBeforePrompting()
        {
            var pipedInput = new[] { "a", "b", "c" };
            new AppRunner<App>()
                .UseSpectreAnsiConsole()
                .UseSpectreArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.DoList)}",
                        PipedInput = pipedInput,
                        OnPrompt = Respond.FailOnPrompt()
                    },
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(pipedInput.ToList())}
                });
        }

        class App
        {
            public void Do([Option] string opt1, string arg1)
            {
            }

            public void DoList(List<string> args)
            {
            }

            public void Secure(string user, Password password)
            {
            }

            public void Flags(
                [Option('a', (string?)null)] bool flagA,
                [Option('b', (string?)null)] bool flagB)
            {
            }

            public void Bool(bool operand1)
            {
            }
        }

        class HierApp
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, 
                int intercept1, 
                [Option(AssignToExecutableSubcommands = true)] int inherited1)
            {
                return next();
            }

            public void Do()
            {
            }
        }
    }
}