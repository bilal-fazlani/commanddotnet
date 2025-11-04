using CommandDotNet.Tests.SourceGen.TestCommands;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.SourceGen.Comparison;

/// <summary>
/// Tests that verify interceptor execution produces identical results
/// </summary>
public class InterceptorTests : ComparisonTestBase
{
    public InterceptorTests(ITestOutputHelper output) : base(output) { }
    [Fact]
    public void Interceptor_WrapsExecution()
    {
        AssertBehaviorMatches<InterceptorCommand>("execute hello");
    }

    [Fact]
    public void Interceptor_Output_ContainsBeforeAndAfter()
    {
        var result = RunWithReflection<InterceptorCommand>("execute test");
        
        // Verify the interceptor ran
        var output = result.Console.AllText();
        output.Should().Contain("[Before Command]");
        output.Should().Contain("Message: test");
        output.Should().Contain("[After Command]");
        
        // Verify generated code produces same result
        AssertBehaviorMatches<InterceptorCommand>("execute test");
    }

    [Fact]
    public void Interceptor_WorksWithMultipleCommands()
    {
        AssertBehaviorMatches<InterceptorCommand>("do-work 5");
    }
}
