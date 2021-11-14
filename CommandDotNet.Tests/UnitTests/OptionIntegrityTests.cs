using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.UnitTests
{
    public class OptionIntegrityTests
    {
        [Fact]
        public void Options_must_have_long_or_short_name()
        {
            Assert.Throws<InvalidConfigurationException>(() => 
                    new Option(null, null, TypeInfo.Flag, ArgumentArity.ExactlyOne))
                .Message.Should().StartWith("a long or short name is required");
        }

        [InlineData('.')]
        [InlineData('-')]
        [InlineData('\b')]
        [InlineData('\t')]
        [InlineData('\n')]
        [Theory]
        public void Options_short_names_must_be_alphanumeric(char shortName)
        {
            Assert.Throws<InvalidConfigurationException>(() =>
                    new Option(null, shortName, TypeInfo.Flag, ArgumentArity.ExactlyOne))
                .Message.Should().StartWith("short name must be alphanumeric but was");
        }

        [InlineData('0')]
        [InlineData('1')]
        [InlineData('9')]
        [InlineData('A')]
        [InlineData('Y')]
        [InlineData('b')]
        [InlineData('z')]
        [Theory]
        public void Options_short_names_can_be(char shortName)
        {
            var option = new Option(null, shortName, TypeInfo.Flag, ArgumentArity.ExactlyOne);
        }
    }
}