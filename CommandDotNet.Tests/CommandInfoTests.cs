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
                ShowParameterInfo = false
            });

            commandInfo.MethodName.Should().Be("CommandWithNoDescription");
            commandInfo.Description.Should().BeNull();
            commandInfo.Parameters.ShouldBeEquivalentTo(new List<CommandParameterInfo>()
            {
                new CommandParameterInfo(new AppSettings())
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    ParameterName = "value",
                    ParameterType = typeof(int),
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
                ShowParameterInfo = false
            });

            commandInfo.MethodName.Should().Be("CommandWithDescription");
            commandInfo.Description.ShouldBeEquivalentTo("some command description");
            commandInfo.Parameters.ShouldBeEquivalentTo(new List<CommandParameterInfo>()
            {
                new CommandParameterInfo(new AppSettings())
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    ParameterName = "value",
                    ParameterType = typeof(int),
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
        
        [Command(Description = "some command description")]
        public void CommandWithDescription([Parameter(Description = "some parameter description")]int value)
        {
            
        }
    }
}