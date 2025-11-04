using CommandDotNet.Tests.SourceGen.TestCommands;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.SourceGen.Comparison;

/// <summary>
/// Tests that verify basic command execution produces identical results
/// between reflection and source-generated code
/// </summary>
public class BasicCommandTests : ComparisonTestBase
{
    public BasicCommandTests(ITestOutputHelper output) : base(output) { }
    [Theory]
    [InlineData("add 5 3", "Result: 8")]
    [InlineData("add -10 10", "Result: 0")]
    [InlineData("subtract 10 3", "Result: 7")]
    [InlineData("multiply 4 5", "Result: 20")]
    public void SimpleCommands_ProduceSameOutput(string args, string expectedOutput)
    {
        AssertOutputContains<Calculator>(args, expectedOutput);
    }

    [Fact]
    public void Calculator_Add_BehaviorMatches()
    {
        AssertBehaviorMatches<Calculator>("add 2 3");
    }

    [Fact]
    public void Calculator_HelpText_Matches()
    {
        AssertBehaviorMatches<Calculator>("--help");
    }

    [Fact]
    public void Calculator_SubcommandHelp_Matches()
    {
        AssertBehaviorMatches<Calculator>("add --help");
    }
}
