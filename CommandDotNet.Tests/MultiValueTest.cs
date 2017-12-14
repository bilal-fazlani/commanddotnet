using System.Collections.Generic;
using CommandDotNet.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class MultiValueTest : TestBase
    {
        public MultiValueTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            
        }
        
        [Fact]
        public void CanRecogniseListWhenPassedInWithMultipleArguments()
        {
            AppRunner<MultiValueApp> appRunner = new AppRunner<MultiValueApp>();
            int exitCode = appRunner.Run(new[] {"accept", "-n", "bilal", "-n", "fazlani"});
            exitCode.Should().Be(2, "length of parameters passed is 2");
        }
    }

    public class MultiValueApp
    {
        [ApplicationMetadata(Name = "accept")]
        public int AcceptList(
            [Argument(ShortName = "n", LongName = "name", Description = "name of person")]
            List<string> names)
        {
            return names.Count;
        }
    }
}