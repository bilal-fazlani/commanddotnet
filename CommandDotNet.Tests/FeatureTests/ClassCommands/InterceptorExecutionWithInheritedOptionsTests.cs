using System.Threading.Tasks;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class InterceptorExecutionWithInheritedOptionsTests
    {
        private readonly ITestOutputHelper _output;

        public InterceptorExecutionWithInheritedOptionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DeclaringCommands_InheritedOptions_NotShown_InHelp()
        {
            new AppRunner<App>()
                .Verify(_output, new Scenario
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll [command] [options]

Options:

  --interceptorOpt  <TEXT>

Commands:

  ChildApp
  Do

Use ""dotnet testhost.dll [command] --help"" for more information about a command."
                    }
                });
        }

        [Fact]
        public void DeclaringCommands_InheritedOptions_NotAccepted()
        {
            // TODO: Does this really make sense?  Should inherited options be specified in either location?  It seems confusing. 
            //       What's the purpose of this feature?
            new AppRunner<App>()
                .Verify(_output, new Scenario
                {
                    WhenArgs = "--interceptorOpt lala --inheritedOpt fishies Do --opt1 5 10",
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "Unrecognized option '--inheritedOpt'" }
                    }
                });
        }

        [Fact]
        public void ExecutableLocalSubcommands_InheritedOptions_AreShown_InHelp()
        {
            new AppRunner<App>()
                .Verify(_output, new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:

  arg1  <NUMBER>

Options:

  --opt1          <NUMBER>

  --inheritedOpt  <TEXT>"
                    }
                });
        }

        [Fact]
        public void ExecutableLocalSubcommands_InheritedOptions_AreAccepted()
        {
            new AppRunner<App>()
                .Verify(_output, new Scenario
                {
                    WhenArgs = "--interceptorOpt lala Do --inheritedOpt fishies --opt1 5 10",
                    Then =
                    {
                        Captured =
                        {
                            new App.InterceptResult
                            {
                                InterceptorOpt = "lala",
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
        public void ExecutableNestedSubcommands_InheritedOptions_AreShown_InHelp()
        {
            new AppRunner<App>()
                .Verify(_output, new Scenario
                {
                    WhenArgs = "ChildApp Do -h",
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll ChildApp Do [options]

Options:

  --inheritedOpt  <TEXT>"
                    }
                });
        }

        [Fact]
        public void ExecutableNestedSubcommands_InheritedOptions_AreAccepted()
        {
            new AppRunner<App>()
                .Verify(_output, new Scenario
                {
                    WhenArgs = "--interceptorOpt lala ChildApp Do --inheritedOpt fishies",
                    Then =
                    {
                        Captured =
                        {
                            new App.InterceptResult
                            {
                                InterceptorOpt = "lala",
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
        public void NonExecutableSubcommands_InheritedOptions_NotShown_InHelp()
        {
            new AppRunner<App>()
                .Verify(_output, new Scenario
                {
                    WhenArgs = "ChildApp -h",
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll ChildApp [command]

Commands:

  Do

Use ""dotnet testhost.dll ChildApp [command] --help"" for more information about a command."
                    }
                });
        }

        [Fact]
        public void NonExecutableSubcommands_InheritedOptions_NotAccepted()
        {
            new AppRunner<App>()
                .Verify(_output, new Scenario
                {
                    WhenArgs = "--interceptorOpt lala ChildApp --inheritedOpt fishies",
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts = { "Unrecognized option '--inheritedOpt'" }
                    }
                });
        }

        class App
        {
            public TestCaptures TestCaptures { get; set; }

            [SubCommand]
            public ChildApp ChildApp { get; set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next,
                string interceptorOpt,
                [Option(AssignToExecutableSubcommands = true)] string inheritedOpt)
            {
                TestCaptures.Capture(new InterceptResult { InheritedOpt = inheritedOpt, InterceptorOpt = interceptorOpt });
                return next();
            }

            public void Do(int arg1, [Option]int opt1)
            {
                TestCaptures.Capture(new DoResult{Arg1 = arg1, Opt1 = opt1});
            }

            public class InterceptResult
            {
                public string InterceptorOpt { get; set; }
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
            public TestCaptures TestCaptures { get; set; }

            public void Do()
            {
                TestCaptures.Capture(new DoResult{Executed = true});
            }

            public class DoResult
            {
                public bool Executed { get; set; }
            }
        }
    }
}