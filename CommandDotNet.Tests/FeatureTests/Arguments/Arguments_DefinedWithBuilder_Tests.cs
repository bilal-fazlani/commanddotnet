using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class Arguments_DefinedWithBuilder_Tests
    {
        public Arguments_DefinedWithBuilder_Tests(ITestOutputHelper testOutputHelper)
        {
            Ambient.Output = testOutputHelper;
        }

        [Fact]
        public void Can_assign_values_to_option_and_operand()
        {
            var result = new AppRunner<App>()
                .OnCommandCreated(cmd =>
                {
                    cmd.AddArgumentNode(new Option("optionA", 'a', TypeInfo.Single<string>(), ArgumentArity.ZeroOrOne));
                    cmd.AddArgumentNode(new Operand("operand1", TypeInfo.Single<string>(), ArgumentArity.ZeroOrOne));
                })
                .RunInMem("Do -a lala fishies");

            result.ExitCode.Should().Be(0);

            var command = result.CommandContext.ParseResult!.TargetCommand;
            var option = command.Find<Option>("optionA");
            option.Should().NotBeNull();
            option!.Value.Should().Be("lala");

            var operand = command.Find<Operand>("operand1");
            operand.Should().NotBeNull();
            operand!.Value.Should().Be("fishies");
        }

        private class App
        {
            public void Do()
            {
            }
        }
    }
}