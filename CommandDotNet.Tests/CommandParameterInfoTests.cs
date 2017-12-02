using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using FluentAssertions.Common;
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
                null, null, null, "--jumped", typeof(bool), false
            },
            new object[]
            {
                "id", CommandOptionType.SingleValue, DBNull.Value, true, "Int64", "Int64 | Required", "Id of person", 
                "Int64 | Required                                  Id of person", "id", "i", "--id | -i", typeof(long), true
            },
            new object[]
            {
                "level", CommandOptionType.SingleValue, DBNull.Value, false, "String",  "String", null,
                null,  null, "l", "-l", typeof(string), false
            },
            new object[]
            {
                "feets", CommandOptionType.SingleValue, DBNull.Value, false, "Int32" , "Int32", null,
                null, "feet", "", "--feet", typeof(int?), false
            },
            new object[]
            {
                "friends", CommandOptionType.MultipleValue, DBNull.Value, false, "String (Multiple)", "String (Multiple)" ,null,
                null, null, null, "--friends", typeof(IEnumerable<string>), false
            },
            new object[]
            {
                "height", CommandOptionType.SingleValue, DBNull.Value, true, "Double",  "Double | Required", null,
                null, null, null, "--height", typeof(double), false
            },
            new object[]
            {
                "log", CommandOptionType.NoValue, DBNull.Value, false, "Flag" , "Flag", null,
                null, null, null, "--log", typeof(bool?), false
            },
            new object[]
            {
                "password", CommandOptionType.SingleValue, DBNull.Value, true, "String" , "String | Required", null,
                null, null, null, "--password", typeof(string), false
            },
            new object[]
            {
                "index", CommandOptionType.SingleValue, 1, false, "Int32",  "Int32 | Default value: 1", null,
                null, null, null, "--index", typeof(int), false
            },
            new object[]
            {
                "name", CommandOptionType.SingleValue, "john", false, "String", "String | Default value: john", "name of person",
                "name of person", null, null, "--name", typeof(string), false
            },
        };

        
        [Theory]
        [MemberData(nameof(TestCases))]
        public void CanIdentifyArguementInfoFromMethods(
            string parameterName,
            CommandOptionType commandOptionType, 
            object defaultValue, 
            bool required,
            string typeDisplayName,
            string parameterDetails,
            string annotatedDescription,
            
            string effectiveDescription,
            string longname,
            string shortname,
            string template,
            Type type,
            bool showParameterDetails)
        {
            MethodInfo methodInfo = typeof(TestApplication).GetMethod("Execute");
            var parameters = methodInfo.GetParameters();
            var parameter = parameters.Single(p => p.Name == parameterName);
            ArguementInfo commandParameterInfo = new ArguementInfo(parameter, 
                new AppSettings{ ShowParameterDetails = showParameterDetails });

            commandParameterInfo.CommandOptionType.Should().Be(commandOptionType);
            commandParameterInfo.DefaultValue.Should().Be(defaultValue);
            commandParameterInfo.Required.Should().Be(required);
            commandParameterInfo.TypeDisplayName.Should().Be(typeDisplayName);
            commandParameterInfo.Details.Should().Be(parameterDetails);
            commandParameterInfo.AnnotatedDescription.Should().Be(annotatedDescription);

            commandParameterInfo.EffectiveDescription.Should().Be(effectiveDescription);
            commandParameterInfo.Template.Should().Be(template);
            commandParameterInfo.Type.Should().Be(type);
        }
        
        [Theory]
        [MemberData(nameof(TestCases))]
        public void CanIdentifyArguementInfoFromConstructor(
            string parameterName,
            CommandOptionType commandOptionType, 
            object defaultValue, 
            bool required,
            string typeDisplayName,
            string parameterDetails,
            string annotatedDescription,
            
            string effectiveDescription,
            string longname,
            string shortname,
            string template,
            Type type,
            bool showParameterDetails)
        {
            ConstructorInfo constructorInfo = typeof(TestApplication)
                .GetConstructors()
                .FirstOrDefault();

            constructorInfo.Should().NotBeNull();
            
            var parameters = constructorInfo.GetParameters();
            var parameter = parameters.Single(p => p.Name == parameterName);
            ArguementInfo commandParameterInfo = new ArguementInfo(parameter, 
                new AppSettings{ ShowParameterDetails = showParameterDetails });

            commandParameterInfo.CommandOptionType.Should().Be(commandOptionType);
            commandParameterInfo.DefaultValue.Should().Be(defaultValue);
            commandParameterInfo.Required.Should().Be(required);
            commandParameterInfo.TypeDisplayName.Should().Be(typeDisplayName);
            commandParameterInfo.Details.Should().Be(parameterDetails);
            commandParameterInfo.AnnotatedDescription.Should().Be(annotatedDescription);

            commandParameterInfo.EffectiveDescription.Should().Be(effectiveDescription);
            commandParameterInfo.Template.Should().Be(template);
            commandParameterInfo.Type.Should().Be(type);
        }
    }
    
    public class TestApplication
    {
        public TestApplication(
            bool jumped, 
            
            [Arguement(ShortName = "i", LongName = "id", Description = "Id of person")]
            long id, 
            
            [Arguement(ShortName = "l")]
            string level, 
            
            [Arguement(LongName = "feet")]
            int? feets, 
            
            IEnumerable<string> friends, 
            
            double height, 
            
            bool? log,
            [Arguement(RequiredString = true)]
            
            string password,
            
            int index = 1,
            
            [Arguement(Description = "name of person")]
            string name = "john")
        {
            
        }
        
        public void Execute(
            bool jumped, 
            
            [Arguement(ShortName = "i", LongName = "id", Description = "Id of person")]
            long id, 
            
            [Arguement(ShortName = "l")]
            string level, 
            
            [Arguement(LongName = "feet")]
            int? feets,
            
            IEnumerable<string> friends, 
            
            double height, 
            
            bool? log,
            [Arguement(RequiredString = true)]
            
            string password,
            
            int index = 1,
            
            [Arguement(Description = "name of person")]
            string name = "john")
        {
            
        }
    }
}