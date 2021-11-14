using System.Threading.Tasks;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PromptForMissingArgumentTests_Hierarchy
    {
        // Adapted in SpectreArgumentPrompterTests.
        // When expectations change here, update above too

        public PromptForMissingArgumentTests_Hierarchy(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void WhenInterceptorOptionMissing_Prompts()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $"{nameof(App.Do)} --inherited1 2",
                        OnPrompt = Respond.WithText("1", prompt => prompt.StartsWith("intercept1"))
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe<App>(1, 2)
                    }
                });
        }

        [Fact]
        public void WhenInheritedOptionMissing_Prompts()
        {
            new AppRunner<App>()
                .UseArgumentPrompter()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = $" --intercept1 1 {nameof(App.Do)}",
                        OnPrompt = Respond.WithText("2", prompt => prompt.StartsWith("inherited1"))
                    },
                    Then =
                    {
                        AssertContext = ctx => ctx.ParamValuesShouldBe<App>(1, 2)
                    }
                });
        }

        class App
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