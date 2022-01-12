using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Prompts;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Prompting
{
    public class PromptForMissingArgumentTests_Password
    {
        // Adapted in SpectreArgumentPrompterTests.
        // When expectations change here, update above too

        public PromptForMissingArgumentTests_Password(ITestOutputHelper output)
        {
            Ambient.Output = output;
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

        class App
        {
            public void Secure(string user, Password password)
            {
            }
        }
    }
}