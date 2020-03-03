using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PromptForMissingArgumentTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PromptForMissingArgumentTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void WhenOperandAndOptionProvided_DoesNotPrompt()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.FailOnPrompt },
                    WhenArgs = $"{nameof(App.Do)} something --opt1 simple",
                    Then =
                    {
                        Outputs = {new App.DoResult
                        {
                            Arg1 = "something",
                            Opt1 = "simple"
                        }},
                        Result = ""
                    }
                });
        }

        [Fact]
        public void WhenOptionMissing_PromptsOnlyForOption()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With("simple", prompt => prompt.StartsWith("opt1")) },
                    WhenArgs = $"{nameof(App.Do)} something",
                    Then =
                    {
                        Outputs = {new App.DoResult
                        {
                            Arg1 = "something",
                            Opt1 = "simple"
                        }},
                        Result = "opt1 (Text): simple"
                    }
                });
        }

        [Fact]
        public void WhenOperandMissing_PromptsOnlyForOperand()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With("something", prompt => prompt.StartsWith("arg1")) },
                    WhenArgs = $"{nameof(App.Do)} --opt1 simple",
                    Then =
                    {
                        Outputs = {new App.DoResult
                        {
                            Arg1 = "something",
                            Opt1 = "simple"
                        }},
                        Result = "arg1 (Text): something"
                    }
                });
        }

        [Fact]
        public void WhenOptionAndOperandMissing_PromptsForBoth()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given =
                    {
                        OnPrompt = Respond.With(
                            new Answer("something", prompt => prompt.StartsWith("arg1")),
                            new Answer("simple", prompt => prompt.StartsWith("opt1"))
                        )
                    },
                    WhenArgs = $"{nameof(App.Do)}",
                    Then =
                    {
                        Outputs =
                        {
                            new App.DoResult
                            {
                                Arg1 = "something",
                                Opt1 = "simple"
                            }
                        },
                        Result = @"arg1 (Text): something
opt1 (Text): simple"
                    }
                });
        }

        [Fact]
        public void WhenOperandListProvided_DoesNotPrompt()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With("yes")},
                    WhenArgs = $"{nameof(App.DoList)} something simple",
                    Then =
                    {
                        Outputs = {new List<string>{"something", "simple"}},
                        Result = ""
                    }
                });
        }

        [Fact]
        public void WhenOperandListMissing_Prompts()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.WithList(new []{"something", "simple"}) },
                    WhenArgs = $"{nameof(App.DoList)}",
                    Then =
                    {
                        Outputs = {new List<string>{"something", "simple"}},
                        Result = @"args (Text) [<enter> once to begin new value. <enter> twice to finish]:
something
simple"
                    }
                });
        }

        [Fact]
        public void ListPrompt_CanIncludeQuotes()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.WithList(new[] { "something", "simple", "'or not'", "\"so simple\"" }) },
                    WhenArgs = $"{nameof(App.DoList)}",
                    Then =
                    {
                        Outputs = {new List<string>{"something", "simple", "'or not'", "\"so simple\""}}
                    }
                });
        }

        [Fact]
        public void WhenInterceptorOptionMissing_Prompts()
        {
            new AppRunner<HierApp>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = {OnPrompt = Respond.With("1", prompt => prompt.StartsWith("intercept1"))},
                    WhenArgs = $"{nameof(HierApp.Do)} --inherited1 2",
                    Then =
                    {
                        Outputs = {new HierApp.InterceptResult {Intercept1 = 1, Inherited1 = 2}}
                    }
                });
        }

        [Fact]
        public void WhenInheritedOptionMissing_Prompts()
        {
            new AppRunner<HierApp>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With("2", prompt => prompt.StartsWith("inherited1")) },
                    WhenArgs = $" --intercept1 1 {nameof(HierApp.Do)}",
                    Then =
                    {
                        Outputs = {new HierApp.InterceptResult {Intercept1 = 1, Inherited1 = 2}}
                    }
                });
        }

        [Fact]
        public void WhenPasswordMissing_PromptMasksInput()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With(
                        new Answer("lala", prompt => prompt.StartsWith("user")), 
                        new Answer("fishies", prompt => prompt.StartsWith("password")))},
                    WhenArgs = $"{nameof(App.Secure)}",
                    Then =
                    {
                        Outputs = { new App.SecureResult{User = "lala", Password = new Password("fishies")}},
                        Result = @"user (Text): lala
password (Text): "
                    }
                });
        }

        [Fact]
        public void WhenPasswordMissing_BackspaceDoesNotRemovePromptText()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With(
                        new Answer("lala", prompt => prompt.StartsWith("user")), 
                        new Answer("fishies\b\b\b\b\b\b\bnew", prompt => prompt.StartsWith("password")))},
                    WhenArgs = $"{nameof(App.Secure)}",
                    Then =
                    {
                        Outputs = { new App.SecureResult{User = "lala", Password = new Password("new")}},
                        Result = @"user (Text): lala
password (Text): "
                    }
                });
        }

        [Fact]
        public void WhenFlagsMissing_DoesNotPrompt()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.FailOnPrompt },
                    WhenArgs = $"{nameof(App.Flags)}",
                    Then = { Result = ""}
                });
        }

        [Fact]
        public void WhenExplicitBoolOptionMissing_Prompts()
        {

            new AppRunner<App>(new AppSettings { BooleanMode = BooleanMode.Explicit })
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With(
                        new Answer("true", prompt => prompt.StartsWith("a ")),
                        new Answer("false", prompt => prompt.StartsWith("b "))
                        )},
                    WhenArgs = $"{nameof(App.Flags)}",
                    Then = { Outputs = { (flagA: true, flagB: false) } }
                });
        }

        [Fact]
        public void WhenBoolOperandMissing_Prompts()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With("true", prompt => prompt.StartsWith("operand1")) },
                    WhenArgs = $"{nameof(App.Bool)}",
                    Then =
                    {
                        Outputs = { true },
                        Result = "operand1 (Boolean): true"
                    }
                });
        }

        [Fact]
        public void CanOverridePromptText()
        {
            new AppRunner<App>()
                .UsePrompting(argumentPromptTextOverride: (ctx, arg) => "lala")
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With("fishies", reuse: true) },
                    WhenArgs = $"{nameof(App.Do)}",
                    Then =
                    {
                        Outputs = { new App.DoResult{Arg1 = "fishies", Opt1 = "fishies"}},
                        Result = @"lala (Text): fishies
lala (Text): fishies"
                    }
                });
        }

        [Fact]
        public void CanFilterListOfArgumentsForPrompting()
        {
            new AppRunner<App>()
                .UsePrompting(argumentFilter: arg => arg.Name == "arg1")
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = {OnPrompt = Respond.With("something", prompt => prompt.StartsWith("arg1"))},
                    WhenArgs = $"{nameof(App.Do)}",
                    Then =
                    {
                        Outputs = {new App.DoResult {Arg1 = "something"}},
                        Result = @"arg1 (Text): something"
                    }
                });
        }

        [Fact]
        public void PipedInputIsEvaluatedBeforePrompting()
        {
            var pipedInput = new[] { "a", "b", "c" };
            new AppRunner<App>()
                .UsePrompting()
                .AppendPipedInputToOperandList()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given =
                    {
                        OnPrompt = Respond.FailOnPrompt,
                        PipedInput = pipedInput
                    },
                    WhenArgs = $"{nameof(App.DoList)}",
                    Then = { Outputs = { pipedInput.ToList() } }
                });
        }

        class App
        {
            private TestOutputs TestOutputs { get; set; }

            public void Do([Option] string opt1, string arg1)
            {
                TestOutputs.Capture(new DoResult{Opt1 = opt1, Arg1 = arg1});
            }

            public void DoList(List<string> args)
            {
                TestOutputs.CaptureIfNotNull(args);
            }

            public void Secure(string user, Password password)
            {
                TestOutputs.Capture(new SecureResult{User = user, Password = password});
            }

            public void Flags(
                [Option(ShortName = "a")] bool flagA,
                [Option(ShortName = "b")] bool flagB)
            {
                TestOutputs.Capture((flagA, flagB));
            }

            public void Bool(bool operand1)
            {
                TestOutputs.Capture(operand1);
            }

            public class DoResult
            {
                public string Opt1; 
                public string Arg1;
            }

            public class SecureResult
            {
                public string User;
                public Password Password;
            }
        }

        class HierApp
        {
            private TestOutputs TestOutputs { get; set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next, int intercept1, [Option(AssignToExecutableSubcommands = true)] int inherited1)
            {
                TestOutputs.Capture(new InterceptResult { Intercept1 = intercept1, Inherited1 = inherited1 });
                return next();
            }

            public void Do() { }

            public class InterceptResult
            {
                public int Intercept1 { get; set; }
                public int Inherited1 { get; set; }
            }
        }
    }
}