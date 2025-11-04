using CommandDotNet.Tests.SourceGen.TestCommands;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.SourceGen.Comparison;

/// <summary>
/// Tests that verify argument models work correctly with generated code
/// </summary>
public class ArgumentModelTests : ComparisonTestBase
{
    public ArgumentModelTests(ITestOutputHelper output) : base(output) { }
    [Fact]
    public void ArgumentModel_BasicUsage()
    {
        AssertBehaviorMatches<ArgumentModelCommand>("process myfile");
    }

    [Fact]
    public void ArgumentModel_WithOptions()
    {
        AssertOutputContains<ArgumentModelCommand>(
            "process test.txt -c 5 -v",
            "Name: test.txt, Count: 5, Verbose: True");
    }

    [Fact]
    public void ArgumentModel_DefaultValues()
    {
        AssertOutputContains<ArgumentModelCommand>(
            "process data",
            "Name: data, Count: 1, Verbose: False");
    }

    [Fact]
    public void ArgumentModel_Help_Matches()
    {
        AssertBehaviorMatches<ArgumentModelCommand>("process --help");
    }
}
