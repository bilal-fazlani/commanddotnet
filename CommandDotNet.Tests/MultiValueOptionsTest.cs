using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class MultiValueOptionsTest : TestBase
    {
        public MultiValueOptionsTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            
        }
        
        [Theory]
        [InlineData("stringList", "-v", "john", "doe")]
        [InlineData("intList", "-v", "3", "5")]
        [InlineData("doubleList", "-v", "4.5", "2.3")]
        [InlineData("enumList", "-v", "Thursday", "Tuesday")]
        [InlineData("charList", "-v", "d", "y")]
        [InlineData("longList", "-v", "123123", "456456534")]
        public void CanRecogniseListWhenPassedInWithMultipleArguments(
            string commandName, string optionName, string option1Value, string option2Value)
        {
            AppRunner<MultiValueApp> appRunner = new AppRunner<MultiValueApp>();
            int exitCode = appRunner.Run(new[] {commandName, optionName, option1Value,
                optionName, option2Value });
            exitCode.Should().Be(2, "length of parameters passed is 2");
        }
    }

    public class MultiValueApp
    {
        public int stringList([Option(ShortName = "v")] List<string> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }

            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int intList([Option(ShortName = "v")] List<int> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int doubleList([Option(ShortName = "v")] List<double> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int longList([Option(ShortName = "v")] List<long> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int enumList([Option(ShortName = "v")] List<DayOfWeek> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int charList([Option(ShortName = "v")] List<char> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
    }
}