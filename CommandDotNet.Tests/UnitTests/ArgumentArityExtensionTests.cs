using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests
{
    public class ArgumentArityExtensionTests
    {
        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, false)]
        [InlineData(0, 2, true)]
        [InlineData(0, int.MaxValue, true)]
        [InlineData(1, 1, false)]
        [InlineData(1, 2, true)]
        [InlineData(1, int.MaxValue, true)]
        [InlineData(int.MaxValue, int.MaxValue, true)]
        public void AllowsMany(int min, int max, bool expected)
        {
            new ArgumentArity(min, max).AllowsMany().Should().Be(expected);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, false)]
        [InlineData(0, 2, false)]
        [InlineData(0, int.MaxValue, false)]
        [InlineData(1, 1, true)]
        [InlineData(1, 2, true)]
        [InlineData(1, int.MaxValue, true)]
        [InlineData(int.MaxValue, int.MaxValue, true)]
        public void RequiresAtLeastOne(int min, int max, bool expected)
        {
            new ArgumentArity(min, max).RequiresAtLeastOne().Should().Be(expected);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, false)]
        [InlineData(0, 5, false)]
        [InlineData(0, int.MaxValue, false)]
        [InlineData(1, 1, true)]
        [InlineData(1, 5, false)]
        [InlineData(1, int.MaxValue, false)]
        [InlineData(2, 2, false)]
        public void RequiresExactlyOne(int min, int max, bool expected)
        {
            new ArgumentArity(min, max).RequiresExactlyOne().Should().Be(expected);
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, false)]
        [InlineData(0, 5, false)]
        [InlineData(0, int.MaxValue, false)]
        [InlineData(1, 1, false)]
        [InlineData(1, 5, false)]
        [InlineData(1, int.MaxValue, false)]
        public void AllowsNone(int min, int max, bool expected)
        {
            new ArgumentArity(min, max).RequiresNone().Should().Be(expected);
        }
    }
}