using System.Threading.Tasks;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands;

public class InterceptorExecutionTests
{
    public InterceptorExecutionTests(ITestOutputHelper output) => Ambient.Output = output;

    [Fact]
    public void InterceptorMethod_WithNoOptions_Help4Parent_NoImpact() =>
        new AppRunner<AppWithNoInterceptorOptions>()
            .Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"Usage: testhost.dll [command]

Commands:

  Do

Use ""testhost.dll [command] --help"" for more information about a command."
                }
            });

    [Fact]
    public void InterceptorMethod_WithNoOptions_Help4Child_NoImpact() =>
        new AppRunner<AppWithNoInterceptorOptions>()
            .Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: testhost.dll Do <arg1>

Arguments:

  arg1  <NUMBER>"
                }
            });

    [Fact]
    public void InterceptorMethod_WithOptions_Help4Parent_ContainsInterceptorOptions() =>
        new AppRunner<AppWithInterceptorOptions>()
            .Verify(new Scenario
            {
                When = {Args = "-h"},
                Then =
                {
                    Output = @"Usage: testhost.dll [command] [options]

Options:

  --stringOpt      <TEXT>

  --skipCmd

  --useReturnCode  <NUMBER>

Commands:

  Do

Use ""testhost.dll [command] --help"" for more information about a command."
                }
            });

    [Fact]
    public void InterceptorMethod_WithOptions_Help4Child_NoImpact() =>
        new AppRunner<AppWithInterceptorOptions>()
            .Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: testhost.dll Do <arg1>

Arguments:

  arg1  <NUMBER>"
                }
            });

    [Fact]
    public void InterceptorMethod_WithNoOptions_IsDetectedAndUsed() =>
        new AppRunner<AppWithNoInterceptorOptions>()
            .TrackingInvocations()
            .Verify(new Scenario
            {
                When = {Args = "Do 1"},
                Then =
                {
                    AssertContext = ctx =>
                    {
                        ctx.GetInterceptorInvocationInfo<AppWithNoInterceptorOptions>().WasInvoked.Should().BeTrue();
                        ctx.GetCommandInvocationInfo().WasInvoked.Should().BeTrue();
                        ctx.ParamValuesShouldBe(1);
                    }
                }
            });

    [Fact]
    public void InterceptorMethod_WithOptions_IsDetectedAndUsed() =>
        new AppRunner<AppWithInterceptorOptions>()
            .TrackingInvocations()
            .Verify(new Scenario
            {
                When = {Args = "--stringOpt lala Do 1"},
                Then =
                {
                    AssertContext = ctx =>
                    {
                        ctx.GetInterceptorInvocationInfo<AppWithInterceptorOptions>().WasInvoked.Should().BeTrue();
                        ctx.ParamValuesShouldBe<AppWithInterceptorOptions>(new InterceptOptions{stringOpt = "lala"});
                        ctx.GetCommandInvocationInfo().WasInvoked.Should().BeTrue();
                        ctx.ParamValuesShouldBe(1);
                    }
                }
            });

    [Fact]
    public void InterceptorMethod_CanBypassNextDelegate() =>
        new AppRunner<AppWithInterceptorOptions>()
            .TrackingInvocations()
            .Verify(new Scenario
            {
                When = {Args = " --skipCmd Do 1"},
                Then =
                {
                    AssertContext = ctx =>
                    {
                        ctx.GetInterceptorInvocationInfo<AppWithInterceptorOptions>().WasInvoked.Should().BeTrue();
                        ctx.ParamValuesShouldBe<AppWithInterceptorOptions>(new InterceptOptions{skipCmd = true});
                        ctx.GetCommandInvocationInfo().WasInvoked.Should().BeFalse();
                        ctx.ParamValuesShouldBe(1);
                    }
                }
            });

    [Fact]
    public void InterceptorMethod_CanChangeReturnCode() =>
        new AppRunner<AppWithInterceptorOptions>()
            .TrackingInvocations()
            .Verify(new Scenario
            {
                When = {Args = " --useReturnCode 5 Do 1"},
                Then =
                {
                    ExitCode = 5,
                    AssertContext = ctx =>
                    {
                        ctx.GetInterceptorInvocationInfo<AppWithInterceptorOptions>().WasInvoked.Should().BeTrue();
                        ctx.ParamValuesShouldBe<AppWithInterceptorOptions>(new InterceptOptions{useReturnCode = 5});
                        ctx.GetCommandInvocationInfo().WasInvoked.Should().BeTrue();
                        ctx.ParamValuesShouldBe(1);
                    }
                }
            });

    [UsedImplicitly]
    class AppWithNoInterceptorOptions
    {
        public Task<int> Intercept(InterceptorExecutionDelegate next)
        {
            return next();
        }

        public void Do(int arg1)
        {
        }
    }

    [UsedImplicitly]
    class AppWithInterceptorOptions
    {
        public Task<int> Intercept(InterceptorExecutionDelegate next,
            InterceptOptions interceptOptions)
        {
            if (interceptOptions.skipCmd)
            {
                return ExitCodes.SuccessAsync;
            }

            var returnCode = next();
            return interceptOptions.useReturnCode.HasValue
                ? Task.FromResult(interceptOptions.useReturnCode.Value)
                : returnCode;
        }

        public void Do(int arg1)
        {
        }
    }

    public class InterceptOptions : IArgumentModel
    {
        public string? stringOpt { get; set; }
        public bool skipCmd { get; set; }
        public int? useReturnCode { get; set; }
    }
}