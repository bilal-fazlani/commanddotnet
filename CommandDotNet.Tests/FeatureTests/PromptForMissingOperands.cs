using System.Collections.Generic;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class PromptForMissingOperands
    {
        private static readonly AppSettings PromptingEnabled = TestAppSettings.TestDefault.Clone(s => s.PromptForMissingOperands = true);

        private readonly ITestOutputHelper _output;

        public PromptForMissingOperands(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void PromptingEnabled_WhenOperandProvidedButMissingOption_DoesNotPrompt()
        {
            var scenario = new Scenario
            {
                Given = {OnReadLine = c => "yes"},
                WhenArgs = "Do something",
                Then =
                {
                    Outputs = {new App.DoResult
                    {
                        arg1 = "something"
                    }}
                }
            };

            VerifyEnabledScenarios(scenario);
        }

        [Fact]
        public void PromptingEnabled_WhenOperandAndOptionProvided_DoesNotPrompt()
        {
            var scenario = new Scenario
            {
                Given = { OnReadLine = c => "yes" },
                WhenArgs = "Do something --opt1 simple",
                Then =
                {
                    Outputs = {new App.DoResult
                    {
                        arg1 = "something",
                        opt1 = "simple"
                    }}
                }
            };

            VerifyEnabledScenarios(scenario);
        }

        [Fact]
        public void PromptingEnabled_WhenMissingOperand_DoesPrompt()
        {
            var scenario = new Scenario
            {
                Given = { OnReadLine = c => "yes" },
                WhenArgs = "Do",
                Then =
                {
                    Outputs = {new App.DoResult
                    {
                        arg1 = "yes"
                    }}
                }
            };

            VerifyEnabledScenarios(scenario);
        }

        [Fact]
        public void PromptingDisabled_WhenMissingOperand_DoesNotPrompt()
        {
            var scenario = new Scenario
            {
                Given = { OnReadLine = c => "yes" },
                WhenArgs = "Do",
                Then =
                {
                    Outputs = {new App.DoResult()}
                }
            };

            new AppRunner<App>()
                .VerifyScenario(_output, scenario);
        }

        [Fact]
        public void PromptingEnabled_WhenOperandListProvided_DoesNotPrompt()
        {
            var scenario = new Scenario
            {
                Given = { OnReadLine = c => "yes" },
                WhenArgs = "DoList something simple",
                Then =
                {
                    Outputs = {new List<string>{"something", "simple"}}
                }
            };

            VerifyEnabledScenarios(scenario);
        }

        [Fact]
        public void PromptingEnabled_WhenMissingOperandList_DoesPrompt()
        {
            var scenario = new Scenario
            {
                Given = { OnReadLine = c => "something simple" },
                WhenArgs = "DoList",
                Then =
                {
                    Outputs = {new List<string>{"something", "simple"}}
                }
            };

            VerifyEnabledScenarios(scenario);
        }

        private void VerifyEnabledScenarios(Scenario scenario)
        {
            new AppRunner<App>()
                .UsePromptForMissingOperands()
                .VerifyScenario(_output, scenario);

            new AppRunner<App>(PromptingEnabled)
                .UseBackwardsCompatibilityMode()
                .VerifyScenario(_output, scenario);
        }

        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public void Do([Option] string opt1, string arg1)
            {
                TestOutputs.Capture(new DoResult{opt1 = opt1, arg1 = arg1});
            }

            public void DoList(List<string> args)
            {
                TestOutputs.Capture(args);
            }

            public class DoResult
            {
                public string opt1; 
                public string arg1;
            }
        }
    }
}