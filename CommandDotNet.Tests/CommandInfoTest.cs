using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests
{
    public class CommandInfoTest
    {
        [Fact]
        public void CanReadCommandNames()
        {
            MethodInfo methodInfo = typeof(FooCommand).GetMethod("Download");
            CommandInfo commandInfo = new CommandInfo(methodInfo, new AppSettings());
            
            commandInfo.Name.Should().Be("Hello");
            commandInfo.Description.Should().Be("Hello Service");
        }
        
        [Fact]
        public void CanCreateCommandNames()
        {
            MethodInfo methodInfo = typeof(FooCommand).GetMethod("DownloadNew");
            CommandInfo commandInfo = new CommandInfo(methodInfo, new AppSettings()
            {
                Case = Case.KebabCase
            });
            
            commandInfo.Name.Should().Be("download-new");
            commandInfo.Description.Should().Be(null);
        }
    }
    
    [ApplicationMetadata(Name = "Foo", Description = "Foo Service")]
    public class FooCommand
    {
        [ApplicationMetadata(Name = "Hello", Description = "Hello Service")]
        public async Task<bool> Download([Option] string provider = "abc")
        {
            return await Task.FromResult(true);
        }
        
        public async Task<bool> DownloadNew([Option] string provider = "abc")
        {
            return await Task.FromResult(true);;
        }
    }
}