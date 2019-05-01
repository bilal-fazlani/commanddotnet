using System;
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
    public class CommandInfoTests : TestBase
    {
        public CommandInfoTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanIdentifyCommandInfoWithoutDescription()
        {
            MethodInfo methodInfo = typeof(CommandInfoTestsApplication).GetMethod("CommandWithNoDescription");
            
            CommandInfo commandInfo = new CommandInfo(methodInfo, new AppSettings
            {
                MethodArgumentMode = ArgumentMode.Option
            });

            commandInfo.Name.Should().Be("CommandWithNoDescription");
            commandInfo.Description.Should().BeNull();
            
            var argumentInfo = commandInfo.Arguments.Cast<CommandOptionInfo>().Single();
            
            argumentInfo.CommandOptionType.Should().Be(CommandOptionType.SingleValue);
            argumentInfo.DefaultValue.Should().Be(DBNull.Value);
            argumentInfo.Type.Should().Be(typeof(int));
            argumentInfo.UnderlyingType.Should().Be(typeof(int));
            argumentInfo.TypeDisplayName.Should().Be(Constants.TypeDisplayNames.Number);
            argumentInfo.AnnotatedDescription.Should().Be(null);
            argumentInfo.Template.Should().Be("--value");
            argumentInfo.BooleanMode.Should().Be(BooleanMode.Implicit);
            argumentInfo.PropertyOrArgumentName.Should().Be("value");
        }
        
        [Fact]
        public void CanIdentifyCommandInfoWithDescriptionAndName()
        {
            MethodInfo methodInfo = typeof(CommandInfoTestsApplication).GetMethod("CommandWithDescriptionAndName");
            CommandInfo commandInfo = new CommandInfo(methodInfo,new AppSettings
            {
                MethodArgumentMode = ArgumentMode.Option
            });

            commandInfo.Name.Should().Be("somecommand");
            commandInfo.ExtendedHelpText.Should().Be("extended help");
            commandInfo.Description.Should().BeEquivalentTo("some command description and name");
            
            var argumentInfo = commandInfo.Arguments.Cast<CommandOptionInfo>().Single();
            
            argumentInfo.CommandOptionType.Should().Be(CommandOptionType.SingleValue);
            argumentInfo.DefaultValue.Should().Be(DBNull.Value);
            argumentInfo.Type.Should().Be(typeof(int));
            argumentInfo.UnderlyingType.Should().Be(typeof(int));
            argumentInfo.TypeDisplayName.Should().Be(Constants.TypeDisplayNames.Number);
            argumentInfo.AnnotatedDescription.Should().Be("some parameter description");
            argumentInfo.Template.Should().Be("--value");
            argumentInfo.BooleanMode.Should().Be(BooleanMode.Implicit);
            argumentInfo.PropertyOrArgumentName.Should().Be("value");
        }
    }
    
    public class CommandInfoTestsApplication
    {        
        public void CommandWithNoDescription(int value)
        {
            
        }
        
        [ApplicationMetadata(
            Description = "some command description and name", 
            Name = "somecommand", 
            ExtendedHelpText = "extended help")]
        public void CommandWithDescriptionAndName([Option(Description = "some parameter description")]int value)
        {
            
        }
    }
}