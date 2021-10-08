using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class TimeDirectiveTests
    {
        public TimeDirectiveTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void Honors_DisableDirectives_AppSetting()
        {
            new AppRunner<App>(new AppSettings { DisableDirectives = true })
                .Verify(
                    new Scenario
                    {
                        When = { Args = "[time] Do" },
                        Then =
                        {
                            ExitCode = 1, // method should have been called
                            OutputContainsTexts = { "Unrecognized command or argument '[time]'" }
                        }
                    });
        }

        [Fact]
        public void Directive_CanBeDisabled()
        {
            new AppRunner<App>()
                .UseTimeDirective()
                .Verify(
                    new Scenario
                    {
                        When = { Args = "[time] Do" },
                        Then =
                        {
                            ExitCode = 5, // method should have been called
                            OutputContainsTexts = {"time: "}
                        }
                    });
        }

        [Fact]
        public void Does_Not_Run_Automatically()
        {
            new AppRunner<App>()
                .UseTimeDirective()
                .Verify(
                    new Scenario
                    {
                        When = { Args = "Do" },
                        Then =
                        {
                            ExitCode = 5, // method should have been called
                            OutputNotContainsTexts = {"time: "}
                        }
                    });
        }


        private class App
        {
            public int Do()
            {
                return 5;
            }
        }
    }
}