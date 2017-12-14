using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using Microsoft.Extensions.CommandLineUtils;
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
                ShowParameterDetails = false
            });

            commandInfo.Name.Should().Be("CommandWithNoDescription");
            commandInfo.Description.Should().BeNull();
            commandInfo.Parameters.ShouldBeEquivalentTo(new List<ArgumentInfo>()
            {
                new ArgumentInfo(new AppSettings())
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    Name = "value",
                    Type = typeof(int),
                    Required = true,
                    EffectiveDescription = null,
                    TypeDisplayName = "Int32",
                    AnnotatedDescription = null,
                    Details = "Int32 | Required",
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
                ShowParameterDetails = true
            });

            commandInfo.Name.Should().Be("somecommand");
            commandInfo.ExtendedHelpText.Should().Be("extended help");
            commandInfo.Description.ShouldBeEquivalentTo("some command description and name");
            commandInfo.Parameters.ShouldBeEquivalentTo(new List<ArgumentInfo>()
            {
                new ArgumentInfo(new AppSettings())
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    Name = "value",
                    Type = typeof(int),
                    Required = true,
                    EffectiveDescription = "Int32 | Required                                  some parameter description",
                    TypeDisplayName = "Int32",
                    AnnotatedDescription = "some parameter description",
                    Details = "Int32 | Required",
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
        public void CommandWithDescriptionAndName([Argument(Description = "some parameter description")]int value)
        {
            
        }
    }
}