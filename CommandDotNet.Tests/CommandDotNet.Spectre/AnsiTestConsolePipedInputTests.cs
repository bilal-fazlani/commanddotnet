using CommandDotNet.Spectre;
using CommandDotNet.Spectre.Testing;
using CommandDotNet.Tests.FeatureTests.Arguments;
using FluentAssertions;
using Spectre.Console;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.Spectre
{
    public class AnsiTestConsolePipedInputTests : AppendPipedInputTestsBase
    {
        // If these work, then onReadLine works as well because PipedInput uses onReadLine

        public AnsiTestConsolePipedInputTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override AppRunner AppRunner<T>() where T : class =>
            new AppRunner<T>()
                .UseSpectreAnsiConsole(new AnsiTestConsole());


        [Fact]
        public void AnsiTestConsole_works_with_TestTools()
        {
            var result = AppRunner<AnsiApp>()
                .RunInMem("Ansi lala");

            result.ExitCode.Should().Be(0);
            result.Console.AllText().Should().Be(@"lala
");
        }

        private class AnsiApp
        {
            public void Ansi(IAnsiConsole ansiConsole, string text)
            {
                ansiConsole.WriteLine(text);
            }
        }
    }
}