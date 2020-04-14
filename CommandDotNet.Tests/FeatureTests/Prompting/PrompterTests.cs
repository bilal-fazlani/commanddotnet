using CommandDotNet.Prompts;
using CommandDotNet.Rendering;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PrompterTests
    {
        private readonly ITestOutputHelper _output;

        public PrompterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void InjectedPrompterCanPromptForValues()
        {
            new AppRunner<App>()
                .UsePrompting()
                .Verify(_output, new Scenario
                {
                    Given = {OnPrompt = Respond.With("who's there")},
                    WhenArgs = "Do",
                    Then =
                    {
                        Output = @"knock knock: who's there
who's there
"
                    }
                });
        }

        public class App
        {
            public void Do(IConsole console, IPrompter prompter)
            {
                var answer = prompter.PromptForValue("knock knock", out bool isCancellationRequested);
                console.Out.WriteLine(answer);
            }
        }
    }
}