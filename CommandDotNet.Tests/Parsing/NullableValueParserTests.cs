using System.Collections.Generic;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;
using CommandDotNet.Parsing;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.Parsing
{
    public class NullableValueParserTests
    {
        [Fact]
        public void CanParseNullableInt()
        {
            NullableValueParser parser = new NullableValueParser(typeof(int), new SingleValueParser(typeof(int)));
            CommandParameterInfo parameterInfo = new CommandParameterInfo(
                typeof(IntPropertyModel).GetProperty("Id"), 
                new AppSettings{ArgumentModelResolver = new CachedArgumentModelResolver()});
            parameterInfo.SetValue(new CommandArgument()
            {
                Values = new List<string>(){"3"},
                MultipleValues = false,
                Name = "Id",
                Description = "Id of employee",
                ShowInHelpText = true
            });
            object value = parser.Parse(parameterInfo);
            value.Should().BeOfType<int>().And.Be(3);
        }

        public class IntPropertyModel : IArgumentModel
        {
            public int? Id { get; set; }
        }
    }
}