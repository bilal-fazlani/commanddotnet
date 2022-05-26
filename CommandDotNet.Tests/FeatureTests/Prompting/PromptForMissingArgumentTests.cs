using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Prompts;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PromptForMissingArgumentTests
    {
        // Adapted in SpectreArgumentPrompterTests.
        // When expectations change here, update above too

        public PromptForMissingArgumentTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void WhenOperandAndOptionProvided_DoesNotPrompt()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)} something --opt1 simple",
                        OnPrompt = Respond.FailOnPrompt()
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
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)} something",
                        OnPrompt = Respond.WithText("simple", prompt => prompt.StartsWith("opt1"))
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("simple", "something"),
                        Output = @"opt1 (Text): simple"
                    }
                });
        }

        [Fact]
        public void WhenOperandMissing_PromptsOnlyForOperand()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)} --opt1 simple",
                        OnPrompt = Respond.WithText("something", prompt => prompt.StartsWith("arg1"))
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("simple", "something"),
                        Output = @"arg1 (Text): something"
                    }
                });
        }

        [Fact]
        public void WhenOptionAndOperandMissing_PromptsForBoth()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.With(
                            new Answer("something", prompt => prompt.StartsWith("arg1")),
                            new Answer("simple", prompt => prompt.StartsWith("opt1"))
                        )
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("simple", "something"),
                        Output = @"arg1 (Text): something
opt1 (Text): simple"
                    }
                });
        }

        [Fact]
        public void WhenOperandListProvided_DoesNotPrompt()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.DoList)} something simple",
                        OnPrompt = Respond.WithText("yes")
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
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.DoList)}",
                        OnPrompt = Respond.WithList(new []{"something", "simple"})
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new List<string>{"something", "simple"}),
                        Output = @"args (Text) [<enter> once to begin new value. <enter> twice to finish]: 
something
simple

"
                    }
                }, config: TestConfig.Default.Where(c => c.SkipTrimEndOfConsoleOutputs = true));
        }

        [Fact]
        public void ListPrompt_CanIncludeQuotes()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.DoList)}",
                        OnPrompt = Respond.WithList(new[] { "something", "simple", "'or not'", "\"so simple\"" })
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(
                            new List<string>{"something", "simple", "'or not'", "\"so simple\""}),
                    }
                });
        }

        [Fact]
        public void WhenPasswordMissing_PromptMasksInput()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Secure)}",
                        OnPrompt = Respond.With(
                            new Answer("lala", prompt => prompt.StartsWith("user")),
                            new Answer("fishies", prompt => prompt.StartsWith("password")))
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("lala", new Password("fishies")),
                        Output = @"user (Text): lala
password (Text):"
                    }
                });
        }

        [Fact]
        public void WhenPasswordMissing_BackspaceDoesNotRemovePromptText()
        {
            // \b is Console for Backspace

            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Secure)}",
                        OnPrompt = Respond.With(
                            new Answer("lala", prompt => prompt.StartsWith("user")),
                            new Answer("fishies\b\b\b\b\b\b\bnew", prompt => prompt.StartsWith("password")))
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("lala", new Password("new")),
                        Output = @"user (Text): lala
password (Text):"
                    }
                });
        }

        [Fact]
        public void WhenFlagsMissing_DoesNotPrompt()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Flags)}",
                        OnPrompt = Respond.FailOnPrompt()
                    },
                    Then = { Output = ""}
                });
        }

        [Fact]
        public void WhenExplicitBoolOptionMissing_Prompts()
        {

            new AppRunner<App>(new AppSettings {Arguments = {BooleanMode = BooleanMode.Explicit }})
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Flags)}",
                        OnPrompt = Respond.With(
                            new Answer("true", prompt => prompt.StartsWith("a ")),
                            new Answer("false", prompt => prompt.StartsWith("b "))
                        )
                    },
                    Then = {AssertContext = ctx => ctx.ParamValuesShouldBe(true, false)}
                });
        }

        [Fact]
        public void WhenBoolOperandMissing_Prompts()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Bool)}",
                        OnPrompt = Respond.WithText("true", prompt => prompt.StartsWith("operand1"))
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(true),
                        Output = @"operand1 (Boolean): true"
                    }
                });
        }

        [Fact]
        public void CanOverridePromptText()
        {
            new AppRunner<App>()
                .UseArgumentPrompter( (c, p) => new ArgumentPrompter(p, (ctx, a) => "lala"))
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.WithText("fishies", reuse: true)
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("fishies", "fishies"),
                        Output = @"lala (Text): fishies
lala (Text): fishies"
                    }
                });
        }

        [Fact]
        public void CanFilterListOfArgumentsForPrompting()
        {
            new AppRunner<App>()
                .UseArgumentPrompter(argumentFilter: arg => arg.Name == "arg1")
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.WithText("something", prompt => prompt.StartsWith("arg1"))
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(null, "something"),
                        ExitCode = 2,
                        Output = @"arg1 (Text): something
opt1 is required"
                    }
                });
        }

        [Fact]
        public void PipedInputIsEvaluatedBeforePrompting()
        {
            var pipedInput = new[] { "a", "b", "c" };
            new AppRunner<App>()
                .UseArgumentPrompter()
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
    }
}