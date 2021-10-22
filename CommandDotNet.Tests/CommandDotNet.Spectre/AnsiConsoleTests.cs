using CommandDotNet.Rendering;
using CommandDotNet.Spectre;
using FluentAssertions;
using Spectre.Console;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.Spectre
{
    public class AnsiConsoleTests
    {
        public AnsiConsoleTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void AnsiConsole_should_resolve_as_parameter()
        {
            AnsiConsole.Record();
            var result = new AppRunner<App>()
                .UseSpectreAnsiConsole()
                .RunInMem("Ansi lala");

            result.ExitCode.Should().Be(0);
            AnsiConsole.ExportText().Should().Be("lala");
            result.Console.AllText().Should().BeEmpty();
        }

        [Fact]
        public void Console_should_forward_to_AnsiConsole()
        {
            var result = new AppRunner<App>()
                .UseSpectreAnsiConsole()
                .RunInMem("Console lala");

            result.ExitCode.Should().Be(0);
            result.Console.AllText().Should().Be(@"lala
");
            AnsiConsole.ExportText().Should().Be("lala");
        }

        class App
        {
            public void Ansi(IAnsiConsole ansiConsole, string text)
            {
                ansiConsole.WriteLine(text);
            }

            public void Console(IConsole console, string text)
            {
                console.WriteLine(text);
            }
        }
    }
}
