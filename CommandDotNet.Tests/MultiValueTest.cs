using System;
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
            int exitCode = appRunner.Run(new[] {"names", "-n", "bilal", "-n", "fazlani"});
            exitCode.Should().Be(2, "length of parameters passed is 2");
        }
        
        [Fact(Skip = "Not ready yet")]
        public void CanRecogniseListWhenPassedInWithCommaSeparatedValues()
        {
            AppRunner<MultiValueApp> appRunner = new AppRunner<MultiValueApp>();
            int exitCode = appRunner.Run(new[] {"cities", "-c", "mumbai, pune, bangalore"});
            exitCode.Should().Be(3, "length of parameters passed is 3");
        }
    }

    public class MultiValueApp
    {
        [ApplicationMetadata(Name = "names")]
        public int NamesList(
            [Option(ShortName = "n", LongName = "name", Description = "names of people")]
            List<string> names)
        {
            foreach (var name in names)
            {
                Console.WriteLine(name);
            }
            return names.Count;
        }
        
        [ApplicationMetadata(Name = "cities")]
        public int CitiesList(
            [Option(ShortName = "c", LongName = "city", Description = "nams of cities")]
            List<string> cities)
        {
            foreach (var city in cities)
            {
                Console.WriteLine(city);
            }
            return cities.Count;
        }
    }
}