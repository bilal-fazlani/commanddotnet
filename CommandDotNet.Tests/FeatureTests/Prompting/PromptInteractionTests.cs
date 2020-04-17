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
        private readonly ITestOutputHelper _output;

        public PromptInteractionTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CtrlC_ExitsApp()
        {
            new AppRunner<App>()
                .UsePrompting()
                .Verify(_output, new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.With(new Answer("part".ToConsoleKeyInfos().AppendCtrlCKey()))
                    },
                    Then =
                    {
                        Output = @"arg1 (Text): part
"
                    }
                });
        }

        [Fact]
        public void SingleEscape_ClearsPrompt()
        {
            var arg1Answer = "take1".ToConsoleKeyInfos().AppendEscapeKey().Concat("take2".ToConsoleKeyInfos());

            new AppRunner<App>()
                .UsePrompting()
                .Verify(_output, new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.With(
                            new Answer(arg1Answer, prompt => prompt.StartsWith("arg1")),
                            new TextAnswer("simple", prompt => prompt.StartsWith("opt1")))
                    },
                    Then =
                    {
                        Captured =
                        {
                            new App.DoResult
                            {
                                Arg1 = "take2",
                                Opt1 = "simple"
                            }
                        },
                        Output = @"arg1 (Text): take2
opt1 (Text): simple
"
                    }
                });
        }

        [Fact]
        public void DoubleEscape_ExitsPrompt()
        {
            new AppRunner<App>()
                .UsePrompting()
                .Verify(_output, new Scenario
                {
                    When=
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.With(
                            new Answer("take1".ToConsoleKeyInfos().AppendEscapeKey().AppendEscapeKey(), prompt => prompt.StartsWith("arg1")),
                            new TextAnswer("not-used", prompt => prompt.StartsWith("arg1")))
                    },
                    Then =
                    {
                        Output = @"arg1 (Text): 
opt1 (Text): 
"
                    }
                });
        }

        [Fact]
        public void BackspaceRemovesCharactersButNotPrompt()
        {
            new AppRunner<App>()
                .UsePrompting()
                .Verify(_output, new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.WithText("yes\b\b\b\bno\b\b\bmaybe", reuse: true)
                    },
                    Then =
                    {
                        Captured = { new App.DoResult{Arg1 = "maybe", Opt1 = "maybe"}},
                        Output = @"arg1 (Text): maybe
opt1 (Text): maybe
"
                    }
                });
        }

        class App
        {
            private TestCaptures TestCaptures { get; set; }

            public void Do([Option] string opt1, string arg1)
            {
                TestCaptures.Capture(new DoResult { Opt1 = opt1, Arg1 = arg1 });
            }

            public class DoResult
            {
                public string Opt1;
                public string Arg1;
            }
        }
    }
}