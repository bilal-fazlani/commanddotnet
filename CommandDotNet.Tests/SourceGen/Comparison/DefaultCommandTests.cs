using CommandDotNet.Tests.SourceGen.TestCommands;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.SourceGen.Comparison;

/// <summary>
/// Tests that verify [DefaultCommand] attribute is handled correctly
/// </summary>
public class DefaultCommandTests : ComparisonTestBase
{
    public DefaultCommandTests(ITestOutputHelper output) : base(output) { }
    [Fact]
    public void DefaultCommand_ExecutesWithoutCommandName()
    {
        AssertBehaviorMatches<DefaultCommandTest>("test-value");
    }

    [Fact]
    public void DefaultCommand_OutputMatches()
    {
        AssertOutputContains<DefaultCommandTest>("my-input", "Default: my-input");
    }

    [Fact]
    public void OtherCommand_StillAccessible()
    {
        AssertOutputContains<DefaultCommandTest>("other some-value", "Other: some-value");
    }
}
