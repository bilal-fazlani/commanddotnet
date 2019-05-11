using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.HelpTests
{
    public class UsageTests : TestBase
    {
        private List<string> _output = new List<string>();
        
        public UsageTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            base.CaptureOutputLine(CaptureOutput);
        }

        private void CaptureOutput(object sender, string e)
        {
            _output.Add(e);
        }

        [Fact]
        public void UsageShouldPrintSubcommandsInCorrectOrder()
        {
            new AppRunner<HelpApp>().Run("SubHelpCommand", "Print", "--help");
            var helpText = _output.First().Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            var usage = helpText.Single(o => o.StartsWith("Usage")).Substring("Usage: ".Length);
            usage.Should().Be("dotnet testhost.dll SubHelpCommand Print [arguments] [options]");
        }
        
        public class HelpApp
        {
            [SubCommand]
            public SubHelpCommand SubCommand { get; set; }
            
            public class SubHelpCommand
            {
                public void Print(string helpText)
                {
                    
                } 
            }
        }
    }
}