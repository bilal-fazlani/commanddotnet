using System;
using System.Collections.Generic;
using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class MultiValueArgumentsTest : TestBase
    {
        public MultiValueArgumentsTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            
        }
        
        [Theory]
        [InlineData("stringList", "john", "doe")]
        [InlineData("intList", "3", "5")]
        [InlineData("doubleList", "4.5", "2.3")]
        [InlineData("enumList", "Thursday", "Tuesday")]
        [InlineData("charList", "d", "y")]
        [InlineData("longList", "123123", "456456534")]
        public void CanRecogniseListWhenPassedInWithMultipleArguments(
            string commandName, string option1Value, string option2Value)
        {
            AppRunner<MultiValueArgumentApp> appRunner = new AppRunner<MultiValueArgumentApp>();
            int exitCode = appRunner.Run(new[] {commandName, option1Value, option2Value });
            exitCode.Should().Be(2, "length of parameters passed is 2");
        }
    }

    public class MultiValueArgumentApp
    {
        public int stringList(List<string> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }

            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int intList(List<int> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int doubleList(List<double> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int longList(List<long> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int enumList(List<DayOfWeek> values)
        {
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
            values.Should().HaveCount(2);
            values[0].Should().NotBe(values[1]);
            return values.Count;
        }
        
        public int charList(List<char> values)
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