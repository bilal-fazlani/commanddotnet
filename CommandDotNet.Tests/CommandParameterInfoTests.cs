using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace CommandDotNet.Tests
{
    public class CommandParameterInfoTests
    {        
        public static IEnumerable<object[]> TestCases = new[] {
            new object[] { "jumped", CommandOptionType.SingleValue, DBNull.Value, true, "Boolean", "Boolean | Required", null},
            new object[] { "level", CommandOptionType.SingleValue, DBNull.Value, false, "String",  "String", null},
            new object[] { "feets", CommandOptionType.SingleValue, DBNull.Value, false, "Int32" , "Int32", null},
            new object[] { "friends", CommandOptionType.MultipleValue, DBNull.Value, false, "String (Multiple)", "String (Multiple)" ,null},
            new object[] { "height", CommandOptionType.SingleValue, DBNull.Value, true, "Double",  "Double | Required", null},
            new object[] { "log", CommandOptionType.NoValue, DBNull.Value, false, "Flag" , "Flag", null},
            new object[] { "password", CommandOptionType.SingleValue, DBNull.Value, true, "String" , "String | Required", null},
            new object[] { "index", CommandOptionType.SingleValue, 1, false, "Int32",  "Int32 | Default value: 1", null},
            new object[] { "name", CommandOptionType.SingleValue, "john", false, "String", "String | Default value: john", "name of person"},
        };

        
        [Theory]
        [MemberData(nameof(TestCases))]
        public void CanIdentifyCommandParameterInfo(
            string parameterName,
            CommandOptionType commandOptionType, 
            object defaultValue, 
            bool required,
            string typeDisplayName,
            string parameterDetails,
            string annotatedDescription)
        {
            MethodInfo methodInfo = typeof(TestApplication).GetMethod("Execute");
            var parameters = methodInfo.GetParameters();
            var parameter = parameters.Single(p => p.Name == parameterName);
            ArguementInfo commandParameterInfo = new ArguementInfo(parameter, 
                new AppSettings{ ShowParameterDetails = true });

            commandParameterInfo.CommandOptionType.Should().Be(commandOptionType);
            commandParameterInfo.DefaultValue.Should().Be(defaultValue);
            commandParameterInfo.Required.Should().Be(required);
            commandParameterInfo.TypeDisplayName.Should().Be(typeDisplayName);
            commandParameterInfo.Details.Should().Be(parameterDetails);
            commandParameterInfo.AnnotatedDescription.ShouldBeEquivalentTo(annotatedDescription);
        }
    }
    
    public class TestApplication
    {
        public void Execute(bool jumped, string level, int? feets, 
            IEnumerable<string> friends, double height, bool? log,
            [Arguement(RequiredString = true)]string password,
            int index = 1,
            [Arguement(Description = "name of person")] string name = "john")
        {
            
        }
    }
}