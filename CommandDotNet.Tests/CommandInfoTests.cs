using System;
using System.Collections.Generic;
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
                ShowArgumentDetails = false,
                MethodArgumentMode = ArgumentMode.Option
            });

            commandInfo.Name.Should().Be("CommandWithNoDescription");
            commandInfo.Description.Should().BeNull();
            commandInfo.Arguments.ShouldBeEquivalentTo(new List<ArgumentInfo>()
            {
                new CommandOptionInfo(new AppSettings())
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    Type = typeof(int),
//                    Required = true,
                    EffectiveDescription = null,
                    TypeDisplayName = "Int32",
                    AnnotatedDescription = null,
                    Details = "Int32",
                    Template = "--value",
                    BooleanMode = BooleanMode.Implicit
                }
            });
        }
        
        
        [Fact]
        public void CanIdentifyCommandInfoWithDescriptionAndName()
        {
            MethodInfo methodInfo = typeof(CommandInfoTestsApplication).GetMethod("CommandWithDescriptionAndName");
            CommandInfo commandInfo = new CommandInfo(methodInfo,new AppSettings
            {
                ShowArgumentDetails = true,
                MethodArgumentMode = ArgumentMode.Option
            });

            commandInfo.Name.Should().Be("somecommand");
            commandInfo.ExtendedHelpText.Should().Be("extended help");
            commandInfo.Description.ShouldBeEquivalentTo("some command description and name");
            commandInfo.Arguments.ShouldBeEquivalentTo(new List<ArgumentInfo>()
            {
                new CommandOptionInfo(new AppSettings())
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    Type = typeof(int),
                    //Required = true,
                    EffectiveDescription = "Int32                                             some parameter description",
                    TypeDisplayName = "Int32",
                    AnnotatedDescription = "some parameter description",
                    Details = "Int32",
                    Template = "--value",
                    BooleanMode = BooleanMode.Implicit
                }
            });
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