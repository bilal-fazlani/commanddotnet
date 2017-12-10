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
            new object[]
            {
                "jumped", CommandOptionType.SingleValue, DBNull.Value, true, "Boolean", "Boolean | Required", null, 
                null, "--jumped", typeof(bool), false
            },
            new object[]
            {
                "id", CommandOptionType.SingleValue, DBNull.Value, true, "Int64", "Int64 | Required", "Id of person", 
                "Int64 | Required                                  Id of person", "--id | -i", typeof(long), true
            },
            new object[]
            {
                "level", CommandOptionType.SingleValue, DBNull.Value, false, "String",  "String", null,
                null, "-l", typeof(string), false
            },
            new object[]
            {
                "feets", CommandOptionType.SingleValue, DBNull.Value, false, "Int32" , "Int32", null,
                null, "--feet", typeof(int?), false
            },
            new object[]
            {
                "friends", CommandOptionType.MultipleValue, DBNull.Value, false, "String (Multiple)", "String (Multiple)" ,null,
                null, "--friends", typeof(IEnumerable<string>), false
            },
            new object[]
            {
                "height", CommandOptionType.SingleValue, DBNull.Value, true, "Double",  "Double | Required", null,
                null, "--height", typeof(double), false
            },
            new object[]
            {
                "log", CommandOptionType.SingleValue, DBNull.Value, false, "Boolean" , "Boolean", null,
                null, "--log", typeof(bool?), false
            },
            new object[]
            {
                "isVerified", CommandOptionType.SingleValue, DBNull.Value, true, "Boolean" , "Boolean | Required", null,
                null, "--isVerified", typeof(bool), false
            },
            new object[]
            {
                "email", CommandOptionType.NoValue, DBNull.Value, false, "Flag" , "Flag", null,
                null, "--email", typeof(bool), false
            },
            new object[]
            {
                "password", CommandOptionType.SingleValue, DBNull.Value, true, "String" , "String | Required", null,
                null, "--password", typeof(string), false
            },
            new object[]
            {
                "index", CommandOptionType.SingleValue, 1, false, "Int32",  "Int32 | Default value: 1", null,
                null, "--index", typeof(int), false
            },
            new object[]
            {
                "name", CommandOptionType.SingleValue, "john", false, "String", "String | Default value: john", "name of person",
                "name of person", "--name", typeof(string), false
            },
            new object[]
            {
                "category1", CommandOptionType.SingleValue, DBNull.Value, true, "Char", "Char | Required", null,
                "Char | Required".PadRight(Constants.PadLength), "--category1", typeof(char), true
            },
            new object[]
            {
                "category2", CommandOptionType.SingleValue, DBNull.Value, false, "Char", "Char", null,
                "Char".PadRight(Constants.PadLength), "--category2", typeof(char?), true
            },
            new object[]
            {
                "category3", CommandOptionType.SingleValue, 'b', false, "Char", "Char | Default value: b", null,
                "Char | Default value: b".PadRight(Constants.PadLength), "--category3", typeof(char), true
            },
        };

        
        [Theory]
        [MemberData(nameof(TestCases))]
        public void CanIdentifyArgumentInfoFromMethods(
            string parameterName,
            CommandOptionType commandOptionType, 
            object defaultValue, 
            bool required,
            string typeDisplayName,
            string parameterDetails,
            string annotatedDescription,
            
            string effectiveDescription,
            string template,
            Type type,
            bool showParameterDetails)
        {
            MethodInfo methodInfo = typeof(TestApplication).GetMethod("Execute");
            var parameters = methodInfo.GetParameters();
            var parameter = parameters.Single(p => p.Name == parameterName);
            ArgumentInfo commandParameterInfo = new ArgumentInfo(parameter, 
                new AppSettings{ ShowParameterDetails = showParameterDetails });

            commandParameterInfo.CommandOptionType.Should().Be(commandOptionType, nameof(commandOptionType));
            commandParameterInfo.DefaultValue.Should().Be(defaultValue, nameof(defaultValue));
            commandParameterInfo.Required.Should().Be(required, nameof(required));
            commandParameterInfo.TypeDisplayName.Should().Be(typeDisplayName, nameof(typeDisplayName));
            commandParameterInfo.Details.Should().Be(parameterDetails, nameof(parameterDetails));
            commandParameterInfo.AnnotatedDescription.Should().Be(annotatedDescription, nameof(annotatedDescription));

            commandParameterInfo.EffectiveDescription.Should().Be(effectiveDescription, nameof(effectiveDescription));
            commandParameterInfo.Template.Should().Be(template, nameof(template));
            commandParameterInfo.Type.Should().Be(type, nameof(type));
        }
    }
    
    public class TestApplication
    {
        public void Execute(
            bool jumped, 
            
            [Argument(ShortName = "i", LongName = "id", Description = "Id of person")]
            long id, 
            
            [Argument(ShortName = "l")]
            string level, 
            
            [Argument(LongName = "feet")]
            int? feets,
            
            IEnumerable<string> friends, 
            
            double height, 
            
            bool? log,
            
            bool isVerified,
            
            [Argument(Flag = true)]
            bool email,
            
            [Argument(RequiredString = true)]
            string password,
            
            char category1,
            
            char? category2,
            
            char category3 = 'b',
            
            int index = 1,
            
            [Argument(Description = "name of person")]
            string name = "john")
        {
            
        }
    }
}