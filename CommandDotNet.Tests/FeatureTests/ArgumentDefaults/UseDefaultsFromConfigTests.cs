using System;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ArgumentDefaults
{
    public class UseDefaultsFromConfigTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UseDefaultsFromConfigTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GivenDefaultValue_Should_DefaultForArgument()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Do",
                Then = {Outputs = { "red" }}
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => Config("red"))
                .VerifyScenario(_testOutputHelper, scenario);
        }

        [Fact]
        public void GivenDefaultValue_Should_OverrideArgumentDefault()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Default",
                Then = { Outputs = { "red" } }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => Config("red"))
                .VerifyScenario(_testOutputHelper, scenario);
        }

        [Fact]
        public void GivenCsvValue_Should_DefaultForArgument()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Do",
                Then = { Outputs = { "red,blue,green" } }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => Config("red,blue,green"))
                .VerifyScenario(_testOutputHelper, scenario);
        }

        [Fact]
        public void GivenNullValue_Should_Skip()
        {
            // null can indicate the key does not exist so default should not be overridden

            var scenario = new Scenario
            {
                WhenArgs = "Default",
                Then = { Outputs = { "lala" } }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(new Func<IArgument, ArgumentDefault>(arg => null))
                .VerifyScenario(_testOutputHelper, scenario);
        }

        [Fact]
        public void GivenMultipleSources_AllAreUsed()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Multi",
                Then = { Outputs = { new []{"one", "two"}} }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => arg.Name == "first" ? new ArgumentDefault("1", "1", "one") : null)
                .UseDefaultsFromConfig(arg => arg.Name == "second" ? new ArgumentDefault("2", "2", "two") : null)
                .VerifyScenario(_testOutputHelper, scenario);
        }

        [Fact]
        public void GivenMultipleSources_FirstRegisteredWins()
        {
            var scenario = new Scenario
            {
                WhenArgs = "Multi",
                Then = { Outputs = { new[] { "right one", "two" } } }
            };

            new AppRunner<App>()
                .UseDefaultsFromConfig(arg => arg.Name == "first" ? new ArgumentDefault("1", "1", "right one") : null)
                .UseDefaultsFromConfig(arg => arg.Name == "first" ? new ArgumentDefault("1", "1", "wrong one") : null)
                .UseDefaultsFromConfig(arg => arg.Name == "second" ? new ArgumentDefault("2", "2", "two") : null)
                .VerifyScenario(_testOutputHelper, scenario);
        }

        private static ArgumentDefault Config(string value)
        {
            return new ArgumentDefault("test", "key", value);
        }

        public class App
        {
            TestOutputs TestOutputs { get; set; }

            public void Multi([Operand] string first, [Operand] string second)
            {
                TestOutputs.Capture(new[] {first, second});
            }

            public void Do([Operand] string op1)
            {
                TestOutputs.Capture(op1);
            }

            public void List([Operand] string[] ops)
            {
                TestOutputs.Capture(ops);
            }

            public void Default([Operand] string op1 = "lala")
            {
                TestOutputs.CaptureIfNotNull(op1);
            }
        }
    }
}