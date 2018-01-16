using System.Collections.Generic;
using CommandDotNet.Parsing;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.Parsing
{
    public class ParserFactoryTests
    {
        [Fact]
        public void CanCreateNullableParser()
        {
            IParser parser = ParserFactory.CreateInstnace(typeof(int?));
            parser.Should().BeOfType<NullableValueParser>();
        }
        
        [Fact]
        public void CanCreateSingleValueParser()
        {
            IParser parser = ParserFactory.CreateInstnace(typeof(int));
            parser.Should().BeOfType<SingleValueParser>();
        }
        
        [Fact]
        public void CanCreateListParser()
        {
            IParser parser = ParserFactory.CreateInstnace(typeof(List<int>));
            parser.Should().BeOfType<ListParser>();
        }

        [Fact]
        public void CanWorkWithEnums()
        {
            ParserFactory.CreateInstnace(typeof(Time)).Should().BeOfType<SingleValueParser>();
            ParserFactory.CreateInstnace(typeof(Time?)).Should().BeOfType<NullableValueParser>();
            ParserFactory.CreateInstnace(typeof(List<Time>)).Should().BeOfType<ListParser>();
        }
        
        [Fact]
        public void CanWorkWithStrings()
        {
            ParserFactory.CreateInstnace(typeof(string)).Should().BeOfType<SingleValueParser>();
            ParserFactory.CreateInstnace(typeof(List<string>)).Should().BeOfType<ListParser>();
        }
    }
}