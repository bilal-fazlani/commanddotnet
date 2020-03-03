using System.Linq;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PromptInteractionTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PromptInteractionTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CtrlC_ExitsApp()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With(new Answer("part".ToConsoleKeyInfos().AppendCtrlCKey())) },
                    WhenArgs = $"{nameof(App.Do)}",
                    Then =
                    {
                        Result = @"arg1 (Text): part"
                    }
                });
        }

        [Fact]
        public void SingleEscape_ClearsPrompt()
        {
            var arg1Answer = "take1".ToConsoleKeyInfos().AppendEscapeKey().Concat("take2".ToConsoleKeyInfos());

            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With(
                        new Answer(arg1Answer, prompt => prompt.StartsWith("arg1")),
                        new Answer("simple", prompt => prompt.StartsWith("opt1")))
                    },
                    WhenArgs = $"{nameof(App.Do)}",
                    Then =
                    {
                        Outputs =
                        {
                            new App.DoResult
                            {
                                Arg1 = "take2",
                                Opt1 = "simple"
                            }
                        },
                        Result = @"arg1 (Text): take2
opt1 (Text): simple"
                    }
                });
        }

        [Fact]
        public void DoubleEscape_ExitsPrompt()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With(
                        new Answer("take1".ToConsoleKeyInfos().AppendEscapeKey().AppendEscapeKey(), prompt => prompt.StartsWith("arg1")),
                        new Answer("not-used", prompt => prompt.StartsWith("arg1"))) },
                    WhenArgs = $"{nameof(App.Do)}",
                    Then =
                    {
                        Result = @"arg1 (Text):
opt1 (Text):"
                    }
                });
        }

        [Fact]
        public void BackspaceRemovesCharactersButNotPrompt()
        {
            new AppRunner<App>()
                .UsePrompting()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    Given = { OnPrompt = Respond.With("yes\b\b\b\bno\b\b\bmaybe", reuse: true) },
                    WhenArgs = $"{nameof(App.Do)}",
                    Then =
                    {
                        Outputs = { new App.DoResult{Arg1 = "maybe", Opt1 = "maybe"}},
                        Result = @"arg1 (Text): maybe
opt1 (Text): maybe"
                    }
                });
        }

        class App
        {
            private TestOutputs TestOutputs { get; set; }

            public void Do([Option] string opt1, string arg1)
            {
                TestOutputs.Capture(new DoResult { Opt1 = opt1, Arg1 = arg1 });
            }

            public class DoResult
            {
                public string Opt1;
                public string Arg1;
            }
        }
    }
}