using System.Linq;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PromptInteractionTests
    {
        public PromptInteractionTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void CtrlC_ExitsApp()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.With(new Answer("part".ToConsoleKeyInfos().AppendCtrlCKey()))
                    },
                    Then =
                    {
                        Output = @"arg1 (Text): part"
                    }
                });
        }

        [Fact]
        public void SingleEscape_ClearsPrompt()
        {
            var arg1Answer = "take1".ToConsoleKeyInfos().AppendEscapeKey().Concat("take2".ToConsoleKeyInfos());

            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.With(
                            new Answer(arg1Answer, prompt => prompt.StartsWith("arg1")),
                            new Answer("simple", prompt => prompt.StartsWith("opt1")))
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("simple", "take2"),
                        Output = @"arg1 (Text): take2
opt1 (Text): simple"
                    }
                });
        }

        [Fact]
        public void DoubleEscape_ExitsPrompt()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When=
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.With(
                            new Answer("take1".ToConsoleKeyInfos().AppendEscapeKey().AppendEscapeKey(), prompt => prompt.StartsWith("arg1")),
                            new Answer("not-used", prompt => prompt.StartsWith("arg1")))
                    },
                    Then =
                    {
                        ExitCode = 2,
                        Output = @"arg1 (Text): 
opt1 (Text): 
opt1 is required
arg1 is required"
                    }
                });
        }

        [Fact]
        public void BackspaceRemovesCharactersButNotPrompt()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.WithText("yes\b\b\b\bno\b\b\bmaybe", reuse: true)
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe("maybe", "maybe"),
                        Output = @"arg1 (Text): maybe
opt1 (Text): maybe"
                    }
                });
        }

        class App
        {
            public void Do([Option] string opt1, string arg1)
            {
            }
        }
    }
}