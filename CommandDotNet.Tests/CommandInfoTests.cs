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
                    Name = "value",
                    Type = typeof(int),
                    Required = true,
                    Description = null
                }
            });
        }
        
        
        [Fact]
        public void CanIdentifyCommandInfoWithDescription()
        {
            MethodInfo methodInfo = typeof(CommandInfoTestsApplication).GetMethod("CommandWithDescription");
            CommandInfo commandInfo = new CommandInfo(methodInfo,new AppSettings
            {
                ShowParameterDetails = false
            });

            commandInfo.Name.Should().Be("CommandWithDescription");
            commandInfo.Description.ShouldBeEquivalentTo("some command description");
            commandInfo.Parameters.ShouldBeEquivalentTo(new List<ArguementInfo>()
            {
                new ArguementInfo(new AppSettings())
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    Name = "value",
                    Type = typeof(int),
                    Required = true,
                    Description = "some parameter description"
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