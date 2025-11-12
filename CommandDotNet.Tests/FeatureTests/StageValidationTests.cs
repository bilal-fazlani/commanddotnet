using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests;

public class StageValidationTests
{
    private static readonly AppSettings AppSettingsWithValidation = 
        new() { Execution = { EnableStageValidation = true } };

    public StageValidationTests(ITestOutputHelper output) => Ambient.Output = output;

    [Fact]
    public void ByDefault_ValidationMiddleware_IsNotInPipeline()
    {
        new AppRunner<App>().Verify(new Scenario
        {
            When = { Args = "Do value1 value2" },
            Then =
            {
                AssertContext = ctx => ctx.AppConfig.MiddlewarePipeline.Should()
                    .NotContain(p => p.Method.DeclaringType != null && 
                                     p.Method.DeclaringType.Name == nameof(StageValidationMiddleware))
            }
        });
    }

    [Fact]
    public void WhenEnabled_ValidationMiddleware_IsInPipeline()
    {
        new AppRunner<App>(AppSettingsWithValidation).Verify(new Scenario
        {
            When = { Args = "Do value1 value2" },
            Then =
            {
                AssertContext = ctx => ctx.AppConfig.MiddlewarePipeline.Should()
                    .Contain(p => p.Method.Name == nameof(StageValidationMiddleware.ValidatePostPreTokenize))
                    .And.Contain(p => p.Method.Name == nameof(StageValidationMiddleware.ValidatePostTokenize))
                    .And.Contain(p => p.Method.Name == nameof(StageValidationMiddleware.ValidatePostParseInput))
                    .And.Contain(p => p.Method.Name == nameof(StageValidationMiddleware.ValidatePostBindValues))
            }
        });
    }

    [Fact]
    public void WhenEnabled_WithValidState_ValidatesSuccessfully()
    {
        var result = new AppRunner<App>(AppSettingsWithValidation)
            .RunInMem("Do value1 value2");

        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePreTokenize_ValidatesOriginalIsPopulated()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            ctx.Original.Should().NotBeNull("ValidatePreTokenize should ensure Original is populated");
        }, MiddlewareStages.PreTokenize, orderWithinStage: short.MaxValue);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePreTokenize_ValidatesTokensIsPopulated()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            ctx.Tokens.Should().NotBeNull("ValidatePreTokenize should ensure Tokens is populated (first pass)");
        }, MiddlewareStages.PreTokenize, orderWithinStage: short.MaxValue);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePreTokenize_ValidatesAppConfigIsPopulated()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            ctx.AppConfig.Should().NotBeNull("ValidatePreTokenize should ensure AppConfig is populated");
        }, MiddlewareStages.PreTokenize, orderWithinStage: short.MaxValue);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePreTokenize_ValidatesConsoleIsPopulated()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            ctx.Console.Should().NotBeNull("ValidatePreTokenize should ensure Console is populated");
        }, MiddlewareStages.PreTokenize, orderWithinStage: short.MaxValue);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePostTokenize_ValidatesTokensIsPopulated()
    {
        var exceptionThrown = false;
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        // Capture state and verify Tokens is validated
        runner.CaptureState(ctx =>
        {
            // If we get here after ValidatePostTokenize, validation passed
            ctx.Tokens.Should().NotBeNull("ValidatePostTokenize should ensure Tokens is populated");
        }, MiddlewareStages.PostTokenizePreParseInput, orderWithinStage: 100);

        try
        {
            runner.RunInMem("Do value1 value2");
        }
        catch (System.InvalidOperationException ex) when (ex.Message.Contains("Tokens"))
        {
            exceptionThrown = true;
        }

        exceptionThrown.Should().BeFalse("validation should pass when Tokens is properly populated");
    }

    [Fact]
    public void ValidatePostTokenize_ValidatesRootCommandIsPopulated()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            ctx.RootCommand.Should().NotBeNull("ValidatePostTokenize should ensure RootCommand is populated");
            ctx.RootCommand!.Name.Should().NotBeNullOrEmpty("ValidatePostTokenize should ensure RootCommand.Name is populated");
        }, MiddlewareStages.PostTokenizePreParseInput, orderWithinStage: 100);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePostParseInput_ValidatesParseResultIsPopulated()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            ctx.ParseResult.Should().NotBeNull("ValidatePostParseInput should ensure ParseResult is populated");
            ctx.ParseResult!.TargetCommand.Should().NotBeNull("ValidatePostParseInput should ensure TargetCommand is populated");
        }, MiddlewareStages.PostParseInputPreBindValues, orderWithinStage: 100);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePostParseInput_ValidatesInvocationPipelineIsPopulated()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            var steps = ctx.InvocationPipeline.All.ToList();
            steps.Should().NotBeEmpty("ValidatePostParseInput should ensure InvocationPipeline has steps");
            
            foreach (var step in steps)
            {
                step.Command.Should().NotBeNull("ValidatePostParseInput should ensure Command is populated");
                step.Invocation.Should().NotBeNull("ValidatePostParseInput should ensure Invocation is populated");
                step.Instance.Should().BeNull("ValidatePostParseInput should ensure Instance is NOT yet populated");
            }
        }, MiddlewareStages.PostParseInputPreBindValues, orderWithinStage: 100);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePostParseInput_ValidatesArgumentInputValuesAreInitialized()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            var command = ctx.ParseResult!.TargetCommand!;
            foreach (var argument in command.AllArguments(includeInterceptorOptions: true))
            {
                argument.InputValues.Should().NotBeNull(
                    $"ValidatePostParseInput should ensure InputValues is initialized for argument '{argument.Name}'");
            }
        }, MiddlewareStages.PostParseInputPreBindValues, orderWithinStage: 100);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePostBindValues_ValidatesInstanceIsPopulated()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            var steps = ctx.InvocationPipeline.All.ToList();
            foreach (var step in steps)
            {
                step.Instance.Should().NotBeNull(
                    $"ValidatePostBindValues should ensure Instance is populated for command '{step.Command?.Name}'");
            }
        }, MiddlewareStages.PostBindValuesPreInvoke, orderWithinStage: 100);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public void ValidatePostBindValues_ValidatesParameterValuesIsPopulated()
    {
        var runner = new AppRunner<App>(AppSettingsWithValidation);
        
        runner.CaptureState(ctx =>
        {
            var steps = ctx.InvocationPipeline.All.ToList();
            foreach (var step in steps)
            {
                step.Invocation.ParameterValues.Should().NotBeNull(
                    $"ValidatePostBindValues should ensure ParameterValues is populated for command '{step.Command?.Name}'");
                
                var expectedCount = step.Invocation.MethodInfo?.GetParameters().Length ?? 0;
                step.Invocation.ParameterValues.Length.Should().Be(expectedCount,
                    $"ValidatePostBindValues should ensure ParameterValues count matches method parameters for command '{step.Command?.Name}'");
            }
        }, MiddlewareStages.PostBindValuesPreInvoke, orderWithinStage: 100);

        var result = runner.RunInMem("Do value1 value2");
        result.ExitCode.Should().Be(0);
    }

    private class App
    {
        public void Do(string arg1, string arg2)
        {
        }
    }
}
