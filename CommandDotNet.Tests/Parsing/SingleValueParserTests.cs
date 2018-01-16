using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;
using CommandDotNet.Parsing;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.Parsing
{
    public class SingleValueParserTests
    {
        [Theory]
        [InlineData("Int", typeof(int), "3", 3)]
        [InlineData("String", typeof(string), "data", "data")]
        [InlineData("Double", typeof(double), "4.5", 4.5D)]
        [InlineData("Long", typeof(long), "5656565656", 5656565656L)]
        [InlineData("Time", typeof(Time), "Tomorrow", Time.Tomorrow)]
        [InlineData("Char", typeof(char), "g", 'g')]
        public void CanParIntegers(string propertyName, Type valueType, string value, object typedValue)
        {
            SingleValueParser valueParser = new SingleValueParser(valueType);
            PropertyInfo propertyInfo = typeof(PropertyModel).GetProperty(propertyName);
            AppSettings appSettings = new AppSettings();
            CommandOptionInfo commandOptionInfo = new CommandOptionInfo(propertyInfo, appSettings);
            
            //set value
            CommandOption option = new CommandOption("--test", CommandOptionType.SingleValue);
            commandOptionInfo.SetValue(option);
            commandOptionInfo.ValueInfo.Values = new List<string>(){value};
                
            object parsedValue = valueParser.Parse(commandOptionInfo);

            parsedValue.Should().BeOfType(valueType);
            parsedValue.Should().Be(typedValue);
        }
    }
    
    public class PropertyModel
    {
        public int Int { get; set; }
        public string String { get; set; }
        public double Double { get; set; }
        public long Long { get; set; }
        public bool Bool { get; set; }
        public Time Time { get; set; }
        public char Char { get; set; }
    }
}