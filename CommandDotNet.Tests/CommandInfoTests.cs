using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace CommandDotNet.Tests
{
    public class CommandInfoTests
    {
        [Fact]
        public void CanIdentifyCommandInfo()
        {
            MethodInfo methodInfo = typeof(CommandInfoTestsApplication).GetMethod("Execute");
            CommandInfo commandInfo = new CommandInfo(methodInfo);

            commandInfo.MethodName.Should().Be("Execute");
            commandInfo.Parameters.ShouldBeEquivalentTo(new List<CommandParameterInfo>()
            {
                new CommandParameterInfo
                {
                    CommandOptionType = CommandOptionType.SingleValue,
                    DefaultValue = DBNull.Value,
                    ParameterName = "value",
                    ParameterType = typeof(int),
                    Required = true
                }
            });
        }
    }
    
    public class CommandInfoTestsApplication
    {
        public void Execute(int value)
        {
            
        }
    }
}