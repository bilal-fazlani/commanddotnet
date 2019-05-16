using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;
using CommandDotNet.Parsing;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TypeDescriptors;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.Parsing
{
    public class SingleValueParserTests
    {
        private static readonly AppSettings _appSettings = InitAppSettings();

        private static readonly Dictionary<string, CommandOptionInfo> _commandOptionInfos =
            TestFactory.GetArgumentsFromModel<PropertyModel>(_appSettings)
                .Cast<CommandOptionInfo>()
                .ToDictionary(a => a.PropertyOrArgumentName);


        public static TheoryData<string, Type, string, object> Values =>
            new TheoryData<string, Type, string, object>
            {
                {"Int", typeof(int), "3", 3},
                {"String", typeof(string), "data", "data"},
                {"Double", typeof(double), "4.5", 4.5D},
                {"Long", typeof(long), "5656565656", 5656565656L},
                {"Time", typeof(Time), "Tomorrow", Time.Tomorrow},
                {"Char", typeof(char), "g", 'g'},
                {"Decimal", typeof(decimal), "4.5", 4.5m},
                {"Person", typeof(Person), "lala", new Person("lala")},
            };
    
        public class PropertyModel
        {
            public int Int { get; set; }
            public string String { get; set; }
            public double Double { get; set; }
            public long Long { get; set; }
            public bool Bool { get; set; }
            public Time Time { get; set; }
            public char Char { get; set; }
            public decimal Decimal { get; set; }
            public Person Person { get; set; }
        }
        
        [Theory]
        [MemberData(nameof(Values))]
        public void CanParseValues(string propertyName, Type valueType, string value, object typedValue)
        {
            var typeDescriptor = _appSettings.ArgumentTypeDescriptors.GetDescriptor(valueType);
            var valueParser = new SingleValueParser(typeDescriptor);

            var commandOptionInfo = _commandOptionInfos[propertyName].SetValueForTest(value);
            commandOptionInfo.SetValueForTest(value);
            
            object parsedValue = valueParser.Parse(commandOptionInfo);

            parsedValue.Should().BeOfType(valueType);
            parsedValue.Should().Be(typedValue);
        }

        private static AppSettings InitAppSettings()
        {
            var appSettings = new AppSettings();
            appSettings.ArgumentTypeDescriptors.Add(new PersonTypeDescriptor());
            return appSettings;
        }

        public class Person
        {
            public string Name { get; }

            public Person(string name)
            {
                Name = name;
            }

            public override bool Equals(object obj)
            {
                return obj is Person that && Name == that.Name;
            }
        }
        
        public class PersonTypeDescriptor : IArgumentTypeDescriptor
        {
            public bool CanSupport(Type type)
            {
                return type == typeof(Person);
            }

            public string GetDisplayName(ArgumentInfo argumentInfo)
            {
                return "text";
            }

            public object ParseString(ArgumentInfo argumentInfo, string value)
            {
                argumentInfo.Should().NotBeNull();
                return new Person(value);
            }
        }
    }
}