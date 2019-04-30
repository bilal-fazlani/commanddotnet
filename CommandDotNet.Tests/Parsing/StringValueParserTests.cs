using System;
using System.Collections.Generic;
using System.Text;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.Parsing
{
    public class StringValueParserTests : TestBase
    {
        [Theory]
        [InlineData("test config sometext", 0)]
        [InlineData("test config \"sometext (and more)\"", 0)]
        public void TestValues(string input, int expectedCode)
        {
            var app = new AppRunner<StringValueApplication>(new AppSettings(){Case = Case.LowerCase});
            var result = app.Run(input.Split(' '));
            result.Should().Be(expectedCode);
        }

        private class StringValueApplication
        {
            public int Test(
                [Argument(Name = "action")]
                string actionType,
                string text)
            {
                return 0;
            }
        }

        /// <inheritdoc />
        public StringValueParserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }
}
