using System.Threading.Tasks;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class InterceptorExecutionWithInheritedOptionsTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public InterceptorExecutionWithInheritedOptionsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void InheritedOptions_CanBeProvidedInExecutableLocalChildren()
        {
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--nonInheritedOpt lala Do --inheritedOpt fishies --opt1 5 10",
                    Then =
                    {
                        Outputs =
                        {
                            new App.InterceptResult
                            {
                                NonInheritedOpt = "lala",
                                InheritedOpt = "fishies"
                            },
                            new App.DoResult
                            {
                                Arg1 = 10,
                                Opt1 = 5
                            }
                        }
                    }
                });
        }

        [Fact]
        public void InheritedOptions_CanBeProvidedInExecutableNestedChildren()
        {
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--nonInheritedOpt lala ChildApp Do --inheritedOpt fishies",
                    Then =
                    {
                        Outputs =
                        {
                            new App.InterceptResult
                            {
                                NonInheritedOpt = "lala",
                                InheritedOpt = "fishies"
                            },
                            new ChildApp.DoResult
                            {
                                Executed = true
                            }
                        }
                    }
                });
        }

        [Fact]
        public void InheritedOptions_CanBeProvidedWithDeclaringCommand()
        {
            // TODO: Does this really make sense?  Should inherited options be specified in either location?  It seems confusing. 
            //       What's the purpose of this feature?
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--nonInheritedOpt lala --inheritedOpt fishies Do --opt1 5 10",
                    Then =
                    {
                        Outputs =
                        {
                            new App.InterceptResult
                            {
                                NonInheritedOpt = "lala",
                                InheritedOpt = "fishies"
                            },
                            new App.DoResult
                            {
                                Arg1 = 10,
                                Opt1 = 5
                            }
                        }
                    }
                });
        }

        [Fact]
        public void InheritedOptions_CanNotBeProvidedInNonExecutableChildren()
        {
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "--nonInheritedOpt lala ChildApp --inheritedOpt fishies",
                    Then =
                    {
                        ExitCode = 1,
                        ResultsContainsTexts = { "Unrecognized option '--inheritedOpt'" }
                    }
                });
        }

        [Fact]
        public void Help_ShowInheritedOptions_InDeclaringCommand()
        {
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll [command] [options]

Options:

  --nonInheritedOpt  <TEXT>

  --inheritedOpt     <TEXT>

Commands:

  ChildApp
  Do

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                });
        }

        [Fact]
        public void Help_DoNotShowInheritedOptions_InNonExecutableChildren()
        {
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "ChildApp -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll ChildApp [command]

Commands:

  Do

Use ""dotnet testhost.dll ChildApp [command] --help"" for more information about a command."
                    }
                });
        }

        [Fact]
        public void Help_ShowInheritedOptions_InExecutableLocalChildren()
        {
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Result = @"Usage: dotnet testhost.dll Do [arguments] [options]

Arguments:

  arg1  <NUMBER>

Options:

  --opt1          <NUMBER>

  --inheritedOpt  <TEXT>"
                    }
                });
        }

        [Fact]
        public void Help_ShowInheritedOptions_InExecutableNestedChildren()
        {
            new AppRunner<App>()
                .VerifyScenario(_testOutputHelper, new Scenario
            {
                WhenArgs = "ChildApp Do -h",
                Then =
                {
                    Result = @"Usage: dotnet testhost.dll ChildApp Do [options]

Options:

  --inheritedOpt  <TEXT>"
                }
            });
        }

        class App
        {
            public TestOutputs TestOutputs { get; set; }

            [SubCommand]
            public ChildApp ChildApp { get; set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next,
                string nonInheritedOpt,
                [Option(Inherited = true)] string inheritedOpt)
            {
                TestOutputs.Capture(new InterceptResult { InheritedOpt = inheritedOpt, NonInheritedOpt = nonInheritedOpt });
                return next();
            }

            public void Do(int arg1, [Option]int opt1)
            {
                TestOutputs.Capture(new DoResult{Arg1 = arg1, Opt1 = opt1});
            }

            public class InterceptResult
            {
                public string NonInheritedOpt { get; set; }
                public string InheritedOpt { get; set; }
            }

            public class DoResult
            {
                public int Arg1 { get; set; }
                public int Opt1 { get; set; }
            }
        }

        class ChildApp
        {
            public TestOutputs TestOutputs { get; set; }

            public void Do()
            {
                TestOutputs.Capture(new DoResult{Executed = true});
            }

            public class DoResult
            {
                public bool Executed { get; set; }
            }
        }
    }
}