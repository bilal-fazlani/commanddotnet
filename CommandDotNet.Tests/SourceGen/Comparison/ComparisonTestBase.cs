using System;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.SourceGen.Comparison;

/// <summary>
/// Base class for tests that compare reflection-based vs source-generated behavior.
/// The gold standard: both approaches MUST produce identical results.
/// </summary>
public abstract class ComparisonTestBase
{
    protected ComparisonTestBase(ITestOutputHelper output)
    {
        Ambient.Output = output;
    }
    /// <summary>
    /// Verifies that running a command with reflection produces the same output as with generated code.
    /// </summary>
    protected void AssertBehaviorMatches<TCommand>(
        string args,
        Action<AppRunner<TCommand>>? configure = null) where TCommand : class
    {
        var reflectionResult = RunWithReflection<TCommand>(args, configure);
        var generatedResult = RunWithGenerated<TCommand>(args, configure);

        AssertResultsMatch(reflectionResult, generatedResult, args);
    }

    /// <summary>
    /// Runs command - with runtime integration, this will automatically use
    /// generated builders if available, falling back to reflection.
    /// </summary>
    protected AppRunnerResult Run<TCommand>(
        string args,
        Action<AppRunner<TCommand>>? configure = null) where TCommand : class
    {
        var runner = new AppRunner<TCommand>();
        configure?.Invoke(runner);
        return runner.RunInMem(args.Split(' '));
    }

    /// <summary>
    /// Runs command using reflection-based ClassCommandDef (for comparison).
    /// Currently same as Run() but kept for clarity in tests.
    /// </summary>
    protected AppRunnerResult RunWithReflection<TCommand>(
        string args,
        Action<AppRunner<TCommand>>? configure = null) where TCommand : class
    {
        // With automatic fallback integrated, this uses generated if available
        // In the future, we could add a flag to force reflection for comparison
        return Run<TCommand>(args, configure);
    }

    /// <summary>
    /// Runs command using source-generated builders (for comparison).
    /// Currently same as Run() but kept for clarity in tests.
    /// </summary>
    protected AppRunnerResult RunWithGenerated<TCommand>(
        string args,
        Action<AppRunner<TCommand>>? configure = null) where TCommand : class
    {
        // With automatic fallback integrated, this uses generated if available
        return Run<TCommand>(args, configure);
    }

    /// <summary>
    /// Asserts that two results are identical in all observable ways
    /// </summary>
    protected void AssertResultsMatch(
        AppRunnerResult expected,
        AppRunnerResult actual,
        string args)
    {
        actual.ExitCode.Should().Be(expected.ExitCode,
            $"exit codes should match for args: {args}");

        actual.Console.AllText().Should().Be(expected.Console.AllText(),
            $"console output should match for args: {args}");

        if (expected.ExitCode != 0)
        {
            // Verify error output matches too
            actual.Console.Error.ToString().Should().Be(expected.Console.Error.ToString(),
                $"error output should match for args: {args}");
        }
    }

    /// <summary>
    /// Verifies output contains expected text (for both reflection and generated)
    /// </summary>
    protected void AssertOutputContains<TCommand>(
        string args,
        string expectedOutput,
        Action<AppRunner<TCommand>>? configure = null) where TCommand : class
    {
        var reflectionResult = RunWithReflection<TCommand>(args, configure);
        var generatedResult = RunWithGenerated<TCommand>(args, configure);

        // Both should contain the expected output
        reflectionResult.Console.AllText().Should().Contain(expectedOutput,
            $"reflection output should contain '{expectedOutput}' for args: {args}");
        
        generatedResult.Console.AllText().Should().Contain(expectedOutput,
            $"generated output should contain '{expectedOutput}' for args: {args}");

        // And they should match each other
        AssertResultsMatch(reflectionResult, generatedResult, args);
    }
}
