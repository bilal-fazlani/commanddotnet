using CommandDotNet.Attributes;
using CommandDotNet.Models;
using CommandDotNet.Tests.Utils;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.Parsing
{
    public class StringValueParserTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public StringValueParserTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("test config sometext", 0)]
        [InlineData("test config \"sometext (and more)\"", 0)]
        public void TestValues(string input, int expectedCode)
        {
            var result = new AppRunner<App>(new AppSettings {Case = Case.LowerCase})
                .RunInMem(input.Split(' '), _testOutputHelper);

            result.ExitCode.Should().Be(expectedCode);
        }

        private class App
        {
            public int Test(
                [Argument(Name = "action")]
                string actionType,
                string text)
            {
                return 0;
            }
        }
    }
}
