using System.Collections.Generic;
using CommandDotNet.Models;
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
            IParser parser = new ParserFactory(new AppSettings()).CreateInstance(typeof(int?));
            parser.Should().BeOfType<NullableValueParser>();
        }
        
        [Fact]
        public void CanCreateSingleValueParser()
        {
            IParser parser = new ParserFactory(new AppSettings()).CreateInstance(typeof(int));
            parser.Should().BeOfType<SingleValueParser>();
        }
        
        [Fact]
        public void CanCreateListParser()
        {
            IParser parser = new ParserFactory(new AppSettings()).CreateInstance(typeof(List<int>));
            parser.Should().BeOfType<ListParser>();
        }

        [Fact]
        public void CanWorkWithEnums()
        {
            new ParserFactory(new AppSettings()).CreateInstance(typeof(Time)).Should().BeOfType<SingleValueParser>();
            new ParserFactory(new AppSettings()).CreateInstance(typeof(Time?)).Should().BeOfType<NullableValueParser>();
            new ParserFactory(new AppSettings()).CreateInstance(typeof(List<Time>)).Should().BeOfType<ListParser>();
        }
        
        [Fact]
        public void CanWorkWithStrings()
        {
            new ParserFactory(new AppSettings()).CreateInstance(typeof(string)).Should().BeOfType<SingleValueParser>();
            new ParserFactory(new AppSettings()).CreateInstance(typeof(List<string>)).Should().BeOfType<ListParser>();
        }
    }
}