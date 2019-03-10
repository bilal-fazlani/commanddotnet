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
            IParser parser = ParserFactory.CreateInstance(typeof(int?));
            parser.Should().BeOfType<NullableValueParser>();
        }
        
        [Fact]
        public void CanCreateSingleValueParser()
        {
            IParser parser = ParserFactory.CreateInstance(typeof(int));
            parser.Should().BeOfType<SingleValueParser>();
        }
        
        [Fact]
        public void CanCreateListParser()
        {
            IParser parser = ParserFactory.CreateInstance(typeof(List<int>));
            parser.Should().BeOfType<ListParser>();
        }

        [Fact]
        public void CanWorkWithEnums()
        {
            ParserFactory.CreateInstance(typeof(Time)).Should().BeOfType<SingleValueParser>();
            ParserFactory.CreateInstance(typeof(Time?)).Should().BeOfType<NullableValueParser>();
            ParserFactory.CreateInstance(typeof(List<Time>)).Should().BeOfType<ListParser>();
        }
        
        [Fact]
        public void CanWorkWithStrings()
        {
            ParserFactory.CreateInstance(typeof(string)).Should().BeOfType<SingleValueParser>();
            ParserFactory.CreateInstance(typeof(List<string>)).Should().BeOfType<ListParser>();
        }
    }
}