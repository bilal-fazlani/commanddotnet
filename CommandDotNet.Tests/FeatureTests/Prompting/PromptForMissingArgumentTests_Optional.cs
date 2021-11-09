using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PromptForMissingArgumentTests_Optional
    {
        public PromptForMissingArgumentTests_Optional(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void When_OptionAndOperand_Missing_DoesNotPrompt_For_Optional_Parameters()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)}",
                        OnPrompt = Respond.FailOnPrompt()
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(null,null)
                    }
                });
        }

        private class App
        {
            public void Do([Option] string? opt1 = null, string? arg1 = null){}
        }
    }
}