using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace CommandDotNet.Tests
{
    public class CommandParameterInfoTests
    {        
        public static IEnumerable<object[]> TestCases = new[] {
            new object[] { "jumped", CommandOptionType.SingleValue, DBNull.Value, true},
            new object[] { "level", CommandOptionType.SingleValue, DBNull.Value, false},
            new object[] { "feets", CommandOptionType.SingleValue, DBNull.Value, false},
            new object[] { "friends", CommandOptionType.MultipleValue, DBNull.Value, false},
            new object[] { "height", CommandOptionType.SingleValue, DBNull.Value, true},
            new object[] { "log", CommandOptionType.NoValue, DBNull.Value, false},
            new object[] { "index", CommandOptionType.SingleValue, 1, false},
            new object[] { "name", CommandOptionType.SingleValue, "john", false},
        };

        
        [Theory]
        [MemberData(nameof(TestCases))]
        public void CanIdentifyCommandParameterInfo(string parameterName, CommandOptionType commandOptionType, object defaultValue, bool required)
        {
            MethodInfo methodInfo = typeof(TestApplication).GetMethod("Execute");
            var parameters = methodInfo.GetParameters();
            var parameter = parameters.Single(p => p.Name == parameterName);
            CommandParameterInfo commandParameterInfo = new CommandParameterInfo(parameter);

            commandParameterInfo.CommandOptionType.Should().Be(commandOptionType);
            commandParameterInfo.DefaultValue.Should().Be(defaultValue);
            commandParameterInfo.Required.Should().Be(required);
        }
    }
    
    public class TestApplication
    {
        public void Execute(bool jumped, string level, int? feets, 
            IEnumerable<string> friends, double height, bool? log,
            int index = 1, string name = "john")
        {
            
        }
    }
}