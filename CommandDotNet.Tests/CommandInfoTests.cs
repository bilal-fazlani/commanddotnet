using System;
using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace CommandDotNet.Tests
{
    public class CommandInfoTests
    {
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
            commandInfo.Parameters.ShouldBeEquivalentTo(new List<ArguementInfo>()
            {
                new ArguementInfo(new AppSettings())
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    LongName = "value",
                    Type = typeof(int),
                    Required = true,
                    EffectiveDescription = null,
                    TypeDisplayName = "Int32",
                    AnnotatedDescription = null,
                    Details = "Int32 | Required",
                    Template = "--value"
                }
            });
        }
        
        
        [Fact]
        public void CanIdentifyCommandInfoWithDescription()
        {
            MethodInfo methodInfo = typeof(CommandInfoTestsApplication).GetMethod("CommandWithDescription");
            CommandInfo commandInfo = new CommandInfo(methodInfo,new AppSettings
            {
                ShowParameterDetails = true
            });

            commandInfo.Name.Should().Be("CommandWithDescription");
            commandInfo.Description.ShouldBeEquivalentTo("some command description");
            commandInfo.Parameters.ShouldBeEquivalentTo(new List<ArguementInfo>()
            {
                new ArguementInfo(new AppSettings())
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    LongName = "value",
                    Type = typeof(int),
                    Required = true,
                    EffectiveDescription = "Int32 | Required                                  some parameter description",
                    TypeDisplayName = "Int32",
                    AnnotatedDescription = "some parameter description",
                    Details = "Int32 | Required",
                    Template = "--value"
                }
            });
        }
    }
    
    
    public class CommandInfoTestsApplication
    {        
        public void CommandWithNoDescription(int value)
        {
            
        }
        
        [ApplicationMetadata(Description = "some command description")]
        public void CommandWithDescription([Arguement(Description = "some parameter description")]int value)
        {
            
        }
    }
}