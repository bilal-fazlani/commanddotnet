using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.Parsing
{
    public class ArgumentParserTests : TestBase
    {
        /// <inheritdoc />
        public ArgumentParserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData("dotnet example.dll --key=value", new string[] { "dotnet", "example.dll", "--key=value" })]
        [InlineData("test config \"sometext (and more)\"", new string[] { "test", "config", "sometext (and more)" })]
        [InlineData("test config -ab", new string[] { "test", "config", "-a", "-b" })]
        public void TestInputValues(string input, string[] expectedResult)
        {
            var chunks = input.Split(" ");
            var parsed = ArgumentParser.SplitFlags(chunks);
            parsed.Should().BeEquivalentTo(expectedResult);
        }
    }
}