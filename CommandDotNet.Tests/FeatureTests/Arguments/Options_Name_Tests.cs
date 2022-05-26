using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Options_Name_Tests
    {
        private readonly Dictionary<string, Option> _options;

        public Options_Name_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;

            _options = new AppRunner<App>()
                .GetFromContext("Do".SplitArgs(),
                    ctx => ctx.ParseResult!.TargetCommand.Options,
                    middlewareStage: MiddlewareStages.PostParseInputPreBindValues)!
                .ToDictionary(o => o.DefinitionSource!.Split('.').Last());
        }

        [InlineData(DefaultName, DefaultName)]
        [InlineData(LongNameOverride, "longName1")]
        [InlineData(ShortNameOverride, ShortNameOverride)]
        [InlineData(ShortAndLongNameOverride, "longName2")]
        [InlineData(LongNameNull, "c")]
        [InlineData(LongNameEmpty, "d")]
        [Theory]
        public void NameShouldBe(string propertyName, string name)
        {
            var option = _options[propertyName];
            option.Name.Should().Be(name);
        }

        [InlineData(DefaultName, null)]
        [InlineData(LongNameOverride, null)]
        [InlineData(ShortNameOverride, 'a')]
        [InlineData(ShortAndLongNameOverride, 'b')]
        [InlineData(LongNameNull, 'c')]
        [InlineData(LongNameEmpty, 'd')]
        [Theory]
        public void ShortNameShouldBe(string propertyName, char? shortName)
        {
            var option = _options[propertyName];
            option.ShortName.Should().Be(shortName);
        }

        [InlineData(DefaultName, DefaultName)]
        [InlineData(LongNameOverride, "longName1")]
        [InlineData(ShortNameOverride, ShortNameOverride)]
        [InlineData(ShortAndLongNameOverride, "longName2")]
        [InlineData(LongNameNull, null)]
        [InlineData(LongNameEmpty, null)]
        [Theory]
        public void LongNameShouldBe(string propertyName, string longName)
        {
            var option = _options[propertyName];
            option.LongName.Should().Be(longName);
        }

        [Fact]
        public void LongNamesCanBeNumbers()
        {
            var result = new AppRunner<App>()
                .OnCommandCreated(cmd =>
                {
                    if (cmd.Name == nameof(App.Do))
                    {
                        cmd.AddArgumentNode(new Option("123", null, TypeInfo.Single<string>(), ArgumentArity.ZeroOrOne));
                    }
                })
                .RunInMem("Do --123 lala");
            result.ExitCode.Should().Be(0);
            var option = result.CommandContext.ParseResult!.TargetCommand.Find<Option>("123");
            option.Should().NotBeNull();
            option!.Value.Should().Be("lala");
        }

        [Fact]
        public void ShortNamesCanBeNumbers()
        {
            var result = new AppRunner<App>()
                .OnCommandCreated(cmd =>
                {
                    if (cmd.Name == nameof(App.Do))
                    {
                        cmd.AddArgumentNode(new Option(null, '1', TypeInfo.Single<string>(), ArgumentArity.ZeroOrOne));
                    }
                })
                .RunInMem("Do -1 lala");
            result.ExitCode.Should().Be(0);
            var option = result.CommandContext.ParseResult!.TargetCommand.Find<Option>("1");
            option.Should().NotBeNull();
            option!.Value.Should().Be("lala");
        }

        private const string DefaultName = "defaultName";
        private const string LongNameOverride = "longNameOverride";
        private const string ShortNameOverride = "shortNameOverride";
        private const string ShortAndLongNameOverride = "shortAndLongNameOverride";
        private const string LongNameNull = "longNameNull";
        private const string LongNameEmpty = "longNameEmpty";

        class App
        {
            public void Do(
                [Option] string? defaultName,
                [Option("longName1")] string? longNameOverride,
                [Option('a')] string? shortNameOverride,
                [Option('b', "longName2")] string? shortAndLongNameOverride,
                [Option('c', (string?)null)] string? longNameNull,
                [Option('d', "")] string? longNameEmpty)
            {
            }
        }
    }
}