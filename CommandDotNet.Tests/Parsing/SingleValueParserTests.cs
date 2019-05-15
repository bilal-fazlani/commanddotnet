using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;
using CommandDotNet.Parsing;
using CommandDotNet.Tests.Utils;
using CommandDotNet.Tests.Parsing.Models;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.Parsing
{
    public class SingleValueParserTests
    {
        private static AppSettings _appSettings = new AppSettings();

        private static readonly Dictionary<string, CommandOptionInfo> _commandOptionInfos =
            TestFactory.GetArgumentsFromModel<PropertyModel>(_appSettings)
                .Cast<CommandOptionInfo>()
                .ToDictionary(a => a.PropertyOrArgumentName);

        public static TheoryData<string, Type, string, object> Values =>
            new TheoryData<string, Type, string, object>
            {
                {"Int", typeof(int), "3", 3},
                {"NullableInt", typeof(int?), "3", (int?) 3},
                {"String", typeof(string), "data", "data"},
                {"Double", typeof(double), "4.5", 4.5D},
                {"Long", typeof(long), "5656565656", 5656565656L},
                {"Time", typeof(Time), "Tomorrow", Time.Tomorrow},
                {"Char", typeof(char), "g", 'g'},
                {"Decimal", typeof(decimal), "4.5", 4.5m},
                {"Person", typeof(Person), "lala", new Person("lala")},
            };

        [Theory]
        [MemberData(nameof(Values))]
        public void CanParseValues(string propertyName, Type valueType, string stringValue, object typedValue)
        {
            var commandOptionInfo = _commandOptionInfos[propertyName];
            commandOptionInfo.SetValueForTest(stringValue);

            var valueParser = new ParserFactory(_appSettings).CreateInstance(commandOptionInfo);
            valueParser.Should().BeOfType<SingleValueParser>();

            object parsedValue = valueParser.Parse(commandOptionInfo);

            parsedValue.Should().Be(typedValue);

            var underlyingType = Nullable.GetUnderlyingType(valueType);
            if (underlyingType == null)
            {
                parsedValue.Should().BeOfType(valueType);
            }
            else
            {
                parsedValue.Should().BeOfType(underlyingType);
            }
        }
    }
}