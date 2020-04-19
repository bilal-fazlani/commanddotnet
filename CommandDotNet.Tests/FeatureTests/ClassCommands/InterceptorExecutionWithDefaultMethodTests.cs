using System.Threading.Tasks;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class InterceptorExecutionWithDefaultMethodTests
    {
        public InterceptorExecutionWithDefaultMethodTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_Help4Parent_NoImpact()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .Verify(new Scenario
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll [command] [arguments]

Arguments:

  defaultArg  <NUMBER>

Commands:

  Do

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_Help4Child_NoImpact()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .Verify(new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Do [arguments]

Arguments:

  arg1  <NUMBER>
"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_Help4Parent_ContainsInterceptorOptions()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .Verify(new Scenario
                {
                    WhenArgs = "-h",
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll [command] [options] [arguments]

Arguments:

  defaultArg  <NUMBER>

Options:

  --stringOpt  <TEXT>

Options also available for subcommands:

  --skipCmd

  --useReturnCode  <NUMBER>

Commands:

  Do

Use ""dotnet testhost.dll [command] --help"" for more information about a command.
"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_Help4Child_NoImpact()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .Verify(new Scenario
                {
                    WhenArgs = "Do -h",
                    Then =
                    {
                        Output = @"Usage: dotnet testhost.dll Do [options] [arguments]

Arguments:

  arg1  <NUMBER>

Options:

  --stringOpt  <TEXT>
"
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_IsDetectedAndUsed()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .Verify(new Scenario
                {
                    WhenArgs = "Do 1",
                    Then =
                    {
                        Captured = { true, 1 }
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithNoOptions_AndDefaultMethod_IsDetectedAndUsed()
        {
            new AppRunner<AppWithNoInterceptorOptions>()
                .Verify(new Scenario
                {
                    WhenArgs = "1",
                    Then =
                    {
                        Captured = { true, 1 }
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_IsDetectedAndUsed()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .Verify(new Scenario
                {
                    WhenArgs = "--stringOpt lala Do 1",
                    Then =
                    {
                        Captured =
                        {
                            new AppWithInteceptorOptions.InterceptOptions {stringOpt = "lala"},
                            1
                        }
                    }
                });
        }

        [Fact]
        public void InterceptorMethod_WithOptions_AndDefaultMethod_IsDetectedAndUsed()
        {
            new AppRunner<AppWithInteceptorOptions>()
                .Verify(new Scenario
                {
                    WhenArgs = "--stringOpt lala 1",
                    Then =
                    {
                        Captured =
                        {
                            new AppWithInteceptorOptions.InterceptOptions {stringOpt = "lala"},
                            1
                        }
                    }
                });
        }

        class AppWithNoInterceptorOptions
        {
            public TestCaptures TestCaptures { get; set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next)
            {
                TestCaptures.Capture(true);
                return next();
            }

            [DefaultMethod]
            public void Default(int defaultArg)
            {
                TestCaptures.Capture(defaultArg);
            }

            public void Do(int arg1)
            {
                TestCaptures.Capture(arg1);
            }
        }

        class AppWithInteceptorOptions
        {
            public TestCaptures TestCaptures { get; set; }

            public Task<int> Intercept(InterceptorExecutionDelegate next,
                InterceptOptions interceptOptions)
            {
                TestCaptures.Capture(interceptOptions);
                if (interceptOptions.skipCmd)
                {
                    return ExitCodes.Success;
                }

                var returnCode = next();
                return interceptOptions.useReturnCode.HasValue
                    ? Task.FromResult(interceptOptions.useReturnCode.Value)
                    : returnCode;
            }

            [DefaultMethod]
            public void Default(int defaultArg)
            {
                TestCaptures.Capture(defaultArg);
            }

            public void Do(int arg1)
            {
                TestCaptures.Capture(arg1);
            }

            public class InterceptOptions : IArgumentModel
            {
                [Option(AssignToExecutableSubcommands = true)]
                public string stringOpt { get; set; }
                public bool skipCmd { get; set; } = false;
                public int? useReturnCode { get; set; }
            }
        }
    }
}