using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CommandDotNet.Models;
using CommandDotNet.Parsing;
using CommandDotNet.Tests.Parsing.Models;
using CommandDotNet.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests.Parsing
{
    public class ParserFactoryTests
    {
        private static AppSettings _appSettings = new AppSettings().Add<PersonTypeDescriptor>();

        private static readonly Dictionary<string, CommandOptionInfo> _commandOptionInfos =
            TestFactory.GetArgumentsFromModel<PropertyModel>(_appSettings)
                .Cast<CommandOptionInfo>()
                .ToDictionary(a => a.PropertyOrArgumentName);
        

        [Fact]
        public void CanCreateSingleValueParser()
        {
            GetParser(m => m.Int).Should().BeOfType<SingleValueParser>();
            GetParser(m => m.NullableInt).Should().BeOfType<SingleValueParser>();
        }

        [Fact]
        public void CanCreateListParser()
        {
            GetParser(m => m.ListInt).Should().BeOfType<ListParser>();
        }

        [Fact]
        public void CanWorkWithEnums()
        {
            GetParser(m => m.Time).Should().BeOfType<SingleValueParser>();
            GetParser(m => m.NullableTime).Should().BeOfType<SingleValueParser>();
            GetParser(m => m.ListTime).Should().BeOfType<ListParser>();
        }
        
        [Fact]
        public void CanWorkWithStrings()
        {
            GetParser(m => m.String).Should().BeOfType<SingleValueParser>();
            GetParser(m => m.ListString).Should().BeOfType<ListParser>();
        }

        private static IParser GetParser<TP>(Expression<Func<PropertyModel, TP>> propertySelector)
        {
            var propertyName = ((MemberExpression) propertySelector.Body).Member.Name;
            return new ParserFactory(_appSettings).CreateInstance(_commandOptionInfos[propertyName]);
        }
    }
}