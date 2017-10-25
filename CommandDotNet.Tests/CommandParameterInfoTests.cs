using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using FluentAssertions;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace CommandDotNet.Tests
{
    public class CommandParameterInfoTests
    {        
        public static IEnumerable<object[]> TestCases = new[] {
            new object[] { "jumped", CommandOptionType.SingleValue, DBNull.Value, true, null},
            new object[] { "level", CommandOptionType.SingleValue, DBNull.Value, false, null},
            new object[] { "feets", CommandOptionType.SingleValue, DBNull.Value, false, null},
            new object[] { "friends", CommandOptionType.MultipleValue, DBNull.Value, false, null},
            new object[] { "height", CommandOptionType.SingleValue, DBNull.Value, true, null},
            new object[] { "log", CommandOptionType.NoValue, DBNull.Value, false, null},
            new object[] { "index", CommandOptionType.SingleValue, 1, false, null},
            new object[] { "name", CommandOptionType.SingleValue, "john", false, "name of person"},
        };

        
        [Theory]
        [MemberData(nameof(TestCases))]
        public void CanIdentifyCommandParameterInfo(
            string parameterName, 
            CommandOptionType commandOptionType, 
            object defaultValue, 
            bool required,
            string description)
        {
            MethodInfo methodInfo = typeof(TestApplication).GetMethod("Execute");
            var parameters = methodInfo.GetParameters();
            var parameter = parameters.Single(p => p.Name == parameterName);
            CommandParameterInfo commandParameterInfo = new CommandParameterInfo(parameter);

            commandParameterInfo.CommandOptionType.Should().Be(commandOptionType);
            commandParameterInfo.DefaultValue.Should().Be(defaultValue);
            commandParameterInfo.Required.Should().Be(required);
            commandParameterInfo.Description.ShouldBeEquivalentTo(description);
        }
    }
    
    public class TestApplication
    {
        public void Execute(bool jumped, string level, int? feets, 
            IEnumerable<string> friends, double height, bool? log,
            int index = 1, [Parameter(Description = "name of person")] string name = "john")
        {
            
        }
    }
}