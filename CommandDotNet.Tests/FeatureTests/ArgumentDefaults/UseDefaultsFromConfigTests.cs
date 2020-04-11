using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ArgumentDefaults
{
    public class UseDefaultsFromConfigTests
    {
        private readonly ITestOutputHelper _output;

        public UseDefaultsFromConfigTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GivenDefaultValue_Should_DefaultForArgument()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Do",
                Then = {Captured = { "red" }}
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => Config("red"))
                .VerifyScenario(_output, scenario);
        }

        [Fact]
        public void GivenDefaultValue_Should_OverrideArgumentDefault()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Default",
                Then = { Captured = { "red" } }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => Config("red"))
                .VerifyScenario(_output, scenario);
        }

        [Fact]
        public void GivenCsvValue_Should_DefaultForArgument()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Do",
                Then = { Captured = { "red,blue,green" } }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => Config("red,blue,green"))
                .VerifyScenario(_output, scenario);
        }

        [Fact]
        public void GivenNullValue_Should_Skip()
        {
            // null can indicate the key does not exist so default should not be overridden

            var scenario = new Scenario
            {
                WhenArgs = "Default",
                Then = { Captured = { "lala" } }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(new Func<IArgument, ArgumentDefault>(arg => null))
                .VerifyScenario(_output, scenario);
        }

        [Fact]
        public void GivenMultipleSources_AllAreUsed()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Multi",
                Then = { Captured = { new []{"one", "two"}} }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => arg.Name == "first" ? new ArgumentDefault("1", "1", "one") : null)
                .UseDefaultsFromConfig(arg => arg.Name == "second" ? new ArgumentDefault("2", "2", "two") : null)
                .VerifyScenario(_output, scenario);
        }

        [Fact]
        public void GivenMultipleSources_FirstRegisteredWins()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Multi",
                Then = { Captured = { new[] { "right one", "two" } } }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => arg.Name == "first" ? new ArgumentDefault("1", "1", "right one") : null)
                .UseDefaultsFromConfig(arg => arg.Name == "first" ? new ArgumentDefault("1", "1", "wrong one") : null)
                .UseDefaultsFromConfig(arg => arg.Name == "second" ? new ArgumentDefault("2", "2", "two") : null)
                .VerifyScenario(_output, scenario);
        }

        private static ArgumentDefault Config(string value)
        {
            return new ArgumentDefault("test", "key", value);
        }

        public class App
        {
            TestCaptures TestCaptures { get; set; }

            public void Multi([Operand] string first, [Operand] string second)
            {
                TestCaptures.Capture(new[] {first, second});
            }

            public void Do([Operand] string op1)
            {
                TestCaptures.Capture(op1);
            }

            public void List([Operand] string[] ops)
            {
                TestCaptures.Capture(ops);
            }

            public void Default([Operand] string op1 = "lala")
            {
                TestCaptures.CaptureIfNotNull(op1);
            }
        }
    }
}