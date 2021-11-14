using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PromptForMissingArgumentTests_NRT
    {
        public PromptForMissingArgumentTests_NRT(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void When_OptionAndOperand_Missing_DoesNotPrompt_For_NRT_Parameters()
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

        [Fact]
        public void When_OptionAndOperand_Missing_DoesNotPrompt_For_NRT_Properties()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Props)}",
                        OnPrompt = Respond.FailOnPrompt()
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe(new App.Model{Opt1 = null,Arg1 = null})
                    }
                });
        }

        private class App
        {
            public void Do([Option] string? opt1, string? arg1){}

            public void Props(Model model){}

            public class Model : IArgumentModel
            {
                [Option]
                public string? Opt1 { get; set; }
                [Operand]
                public string? Arg1 { get; set; }
            }
        }
    }
}