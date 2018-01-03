using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class CommandParameterInfoTests : TestBase
    {
        public CommandParameterInfoTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        public static IEnumerable<object[]> TestCases = new[] {
            new object[]
            {
                "jumped", CommandOptionType.SingleValue, DBNull.Value, "Boolean", "Boolean", null, 
                null, "--jumped", typeof(bool), false
            },
            new object[]
            {
                "id", CommandOptionType.SingleValue, DBNull.Value, "Int64", "Int64", "Id of person", 
                "Int64".PadRight(Constants.PadLength)+"Id of person", "-i | --id", typeof(long), true
            },
            new object[]
            {
                "level", CommandOptionType.SingleValue, DBNull.Value, "String",  "String", null,
                null, "-l", typeof(string), false
            },
            new object[]
            {
                "feets", CommandOptionType.SingleValue, DBNull.Value, "Int32" , "Int32", null,
                null, "--feet", typeof(int?), false
            },
            new object[]
            {
                "friends", CommandOptionType.MultipleValue, DBNull.Value, "String (Multiple)", "String (Multiple)" ,null,
                null, "--friends", typeof(IEnumerable<string>), false
            },
            new object[]
            {
                "height", CommandOptionType.SingleValue, DBNull.Value, "Double",  "Double", null,
                null, "--height", typeof(double), false
            },
            new object[]
            {
                "log", CommandOptionType.NoValue, DBNull.Value, "Flag" , "Flag", null,
                null, "--log", typeof(bool?), false
            },
            new object[]
            {
                "isVerified", CommandOptionType.NoValue, DBNull.Value, "Flag" , "Flag", null,
                null, "--isVerified", typeof(bool), false
            },
            new object[]
            {
                "email", CommandOptionType.NoValue, DBNull.Value, "Flag" , "Flag", null,
                null, "--email", typeof(bool), false
            },
            new object[]
            {
                "password", CommandOptionType.SingleValue, DBNull.Value, "String" , "String", null,
                null, "--password", typeof(string), false
            },
            new object[]
            {
                "index", CommandOptionType.SingleValue, 1, "Int32",  "Int32 | Default value: 1", null,
                null, "--index", typeof(int), false
            },
            new object[]
            {
                "name", CommandOptionType.SingleValue, "john", "String", "String | Default value: john", "name of person",
                "name of person", "--name", typeof(string), false
            },
            new object[]
            {
                "category1", CommandOptionType.SingleValue, DBNull.Value, "Char", "Char", null,
                "Char".PadRight(Constants.PadLength), "--category1", typeof(char), true
            },
            new object[]
            {
                "category2", CommandOptionType.SingleValue, DBNull.Value, "Char", "Char", null,
                "Char".PadRight(Constants.PadLength), "--category2", typeof(char?), true
            },
            new object[]
            {
                "category3", CommandOptionType.SingleValue, 'b', "Char", "Char | Default value: b", null,
                "Char | Default value: b".PadRight(Constants.PadLength), "--category3", typeof(char), true
            },
        };

        [Theory]
        [MemberData(nameof(TestCases))]
        public void CanIdentifyArgumentInfoFromMethods(
            string parameterName,
            CommandOptionType commandOptionType, 
            object defaultValue, 
            //bool required,
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
            CommandOptionInfo commandParameterInfo = new CommandOptionInfo(parameter, 
                new AppSettings{ ShowArgumentDetails = showParameterDetails, MethodArgumentMode = ArgumentMode.Option});

            commandParameterInfo.CommandOptionType.Should().Be(commandOptionType, nameof(commandOptionType));
            commandParameterInfo.DefaultValue.Should().Be(defaultValue, nameof(defaultValue));
            //commandParameterInfo.Required.Should().Be(required, nameof(required));
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
            [Option(BooleanMode = BooleanMode.Explicit)]
            bool jumped, 
            
            [Option(ShortName = "i", LongName = "id", Description = "Id of person")]
            long id, 
            
            [Option(ShortName = "l")]
            string level, 
            
            [Option(LongName = "feet")]
            int? feets,
            
            IEnumerable<string> friends, 
            
            double height, 
            
            bool? log,
            
            [Option(BooleanMode = BooleanMode.Implicit)]
            bool isVerified,
            
            bool email,
            
            string password,
            
            char category1,
            
            char? category2,
            
            char category3 = 'b',
            
            int index = 1,
            
            [Option(Description = "name of person")]
            string name = "john")
        {
            
        }
    }
}