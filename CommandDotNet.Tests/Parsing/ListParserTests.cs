using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AutoFixture;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;
using CommandDotNet.Parsing;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.Parsing
{
    public class ListParserTests
    {
        [Theory]
        [ClassData(typeof(TestDataGeneratorForListParserTests))]
        public void CanParseLists(string parameterName, Type underLyingType, List<string> stringValues, object typedValue)
        {
            ListParser listParser = new ListParser(underLyingType, new ParserFactory(new AppSettings()).GetSingleValueParser(underLyingType));

            ParameterInfo parameterInfo = typeof(ParameterClass).GetMethod("List").GetParameters()
                .Single(x => x.Name == parameterName);
            
            AppSettings appSettings = new AppSettings();
                
            CommandParameterInfo commandParamterInfo = new CommandParameterInfo(parameterInfo, appSettings);

            commandParamterInfo.SetValue(new CommandArgument()
            {
                Name = parameterName,
                Description = "description",
                MultipleValues = true,
                Values = stringValues
            });
            
            IList parsedValues = listParser.Parse(commandParamterInfo);

            parsedValues.Should().HaveCount(stringValues.Count);

            parsedValues.Should().AllBeOfType(underLyingType);

            for (int i = 0; i < stringValues.Count; i++)
            {
                parsedValues[i].Should().Be(((IList) typedValue)[i]);
            }
        }
    }

    public class TestDataGeneratorForListParserTests : IEnumerable<object[]>
    {
        private List<string> guids = new List<string>
        {
            "10000000-0000-0000-0000-000000000001",
            "20000000-0000-0000-0000-000000000002"
        };
        private List<string> uris = new List<string>
        {
            "http://microsoft.com",
            "http://google.com"
        };
        
        public IEnumerator<object[]> GetEnumerator()
        {
            Dictionary<string,List<string>> stringValues = new Dictionary<string, List<string>>()
            {
                ["integers"] = new List<string>(){"3", "2"},
                ["strings"] = new List<string>(){"random", "text"},
                ["booleans"] = new List<string>(){"false", "true"},
                ["longs"] = new List<string>(){"123232", "23445345"},
                ["doubles"] = new List<string>(){"23", "56.6"},
                ["times"] = new List<string>(){"Now", "Tomorrow"},
                ["chars"] = new List<string>(){"v", "B", "y"},
                ["guids"] = guids,
                ["uris"] = uris
            };

            Dictionary<string, IList> typedValues = new Dictionary<string, IList>()
            {
                ["integers"] = new List<int>() {3, 2},
                ["strings"] = new List<string>() {"random", "text"},
                ["booleans"] = new List<bool>() {false, true},
                ["longs"] = new List<long>() {123232, 23445345},
                ["doubles"] = new List<double>() {23, 56.6},
                ["times"] = new List<Time>() {Time.Now, Time.Tomorrow},
                ["chars"] = new List<char>() {'v', 'B', 'y'},
                ["guids"] = guids.Select(s => new Guid(s)).ToList(),
                ["uris"] = uris.Select(s => new Uri(s)).ToList()
            };
            
            IEnumerable<ParameterInfo> parameters = typeof(ParameterClass).GetMethod("List").GetParameters();
            foreach (ParameterInfo parameterInfo in parameters)
            {
                yield return new object[]
                {
                    parameterInfo.Name,
                    parameterInfo.ParameterType.GetGenericArguments()[0],
                    stringValues[parameterInfo.Name],
                    typedValues[parameterInfo.Name]
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
    public class ParameterClass
    {
        public void List(List<int> integers, List<string> strings, List<bool> booleans,
            List<long> longs, List<double> doubles, List<Time> times, List<char> chars, 
            List<Guid> guids, List<Uri> uris)
        {
                
        }
    }
}