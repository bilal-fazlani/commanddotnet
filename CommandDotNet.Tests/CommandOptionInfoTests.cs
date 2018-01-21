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
    public class CommandOptionInfoTests : TestBase
    {
        public CommandOptionInfoTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        public static IEnumerable<object[]> TestCases = new[] {
            new object[]
            {
                "jumped", CommandOptionType.SingleValue, DBNull.Value, Constants.TypeDisplayNames.Boolean, null, 
                "--jumped", typeof(bool)
            },
            new object[]
            {
                "id", CommandOptionType.SingleValue, DBNull.Value, Constants.TypeDisplayNames.Number,  "Id of person", 
                "-i | --id", typeof(long)
            },
            new object[]
            {
                "level", CommandOptionType.SingleValue, DBNull.Value, Constants.TypeDisplayNames.Text,  null,
                "-l", typeof(string)
            },
            new object[]
            {
                "feets", CommandOptionType.SingleValue, DBNull.Value, Constants.TypeDisplayNames.Number , null,
                "--feet", typeof(int?)
            },
            new object[]
            {
                "friends", CommandOptionType.MultipleValue, DBNull.Value, Constants.TypeDisplayNames.Text, null,
                "--friends", typeof(IEnumerable<string>)
            },
            new object[]
            {
                "height", CommandOptionType.SingleValue, DBNull.Value, Constants.TypeDisplayNames.DecimalNumber, null,
                "--height", typeof(double)
            },
            new object[]
            {
                "log", CommandOptionType.NoValue, DBNull.Value, Constants.TypeDisplayNames.Flag, null,
                "--log", typeof(bool?)
            },
            new object[]
            {
                "isVerified", CommandOptionType.NoValue, DBNull.Value, Constants.TypeDisplayNames.Flag, null,
                "--isVerified", typeof(bool)
            },
            new object[]
            {
                "email", CommandOptionType.NoValue, DBNull.Value, Constants.TypeDisplayNames.Flag, null,
                "--email", typeof(bool)
            },
            new object[]
            {
                "password", CommandOptionType.SingleValue, DBNull.Value, Constants.TypeDisplayNames.Text, null,
                "--password", typeof(string)
            },
            new object[]
            {
                "index", CommandOptionType.SingleValue, 1, Constants.TypeDisplayNames.Number, null,
                "--index", typeof(int)
            },
            new object[]
            {
                "name", CommandOptionType.SingleValue, "john", Constants.TypeDisplayNames.Text, "name of person",
                "--name", typeof(string)
            },
            new object[]
            {
                "category1", CommandOptionType.SingleValue, DBNull.Value, Constants.TypeDisplayNames.Character, null,
                "--category1", typeof(char)
            },
            new object[]
            {
                "category2", CommandOptionType.SingleValue, DBNull.Value, Constants.TypeDisplayNames.Character, null,
                "--category2",typeof(char?)
            },
            new object[]
            {
                "category3", CommandOptionType.SingleValue, 'b', Constants.TypeDisplayNames.Character, null,
                "--category3", typeof(char)
            },
        };

        [Theory]
        [MemberData(nameof(TestCases))]
        public void CanIdentifyArgumentInfoFromMethodsUsingParameterInfo(
            string parameterName,
            CommandOptionType commandOptionType, 
            object defaultValue, 
            string typeDisplayName,
            //string parameterDetails,
            string annotatedDescription,
            
            //string effectiveDescription,
            string template,
            Type type
            //bool showParameterDetails
            )
        {
            MethodInfo methodInfo = typeof(TestApplication).GetMethod("Execute");
            var parameters = methodInfo.GetParameters();
            var parameter = parameters.Single(p => p.Name == parameterName);
            CommandOptionInfo commandParameterInfo = new CommandOptionInfo(parameter, 
                new AppSettings{ MethodArgumentMode = ArgumentMode.Option});

            commandParameterInfo.CommandOptionType.Should().Be(commandOptionType, nameof(commandOptionType));
            commandParameterInfo.DefaultValue.Should().Be(defaultValue, nameof(defaultValue));
            commandParameterInfo.TypeDisplayName.Should().Be(typeDisplayName, nameof(typeDisplayName));
//            commandParameterInfo.Details.Should().Be(parameterDetails, nameof(parameterDetails));
            commandParameterInfo.AnnotatedDescription.Should().Be(annotatedDescription, nameof(annotatedDescription));

//            commandParameterInfo.EffectiveDescription.Should().Be(effectiveDescription, nameof(effectiveDescription));
            commandParameterInfo.Template.Should().Be(template, nameof(template));
            commandParameterInfo.Type.Should().Be(type, nameof(type));
        }

        [Fact]
        public void CanCreateCommandOptionInfoFromPropertyInfo()
        {
            PropertyInfo propertyInfo = typeof(ParameterModel).GetProperty("foreignKeyId");
            CommandOptionInfo commandOptionInfo = new CommandOptionInfo(propertyInfo, new AppSettings());

//            commandOptionInfo.Details.Should().Be("Int32", "Details");
            commandOptionInfo.Template.Should().Be("--foreignKeyId");
            commandOptionInfo.LongName.Should().Be("foreignKeyId");
            commandOptionInfo.TypeDisplayName.Should().Be(Constants.TypeDisplayNames.Number);
//            commandOptionInfo.EffectiveDescription.Should().Be("Int32".PadRight(Constants.PadLength));
            commandOptionInfo.IsMultipleType.Should().BeFalse();
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

    public class ParameterModel
    {
        public int foreignKeyId { get; set; }
    }
}