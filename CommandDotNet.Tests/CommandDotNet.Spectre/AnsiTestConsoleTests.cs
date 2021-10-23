using System;
using CommandDotNet.Extensions;
using CommandDotNet.Rendering;
using CommandDotNet.Spectre;
using CommandDotNet.Spectre.Testing;
using FluentAssertions;
using Spectre.Console;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.CommandDotNet.Spectre
{
    public class AnsiTestConsoleTests
    {
        public AnsiTestConsoleTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void AnsiTestConsole_works_with_TestTools()
        {
            var testConsole = new AnsiTestConsole();
            var result = new AppRunner<App>()
                .UseSpectreAnsiConsole(testConsole)
                .RunInMem("Ansi lala");

            result.ExitCode.Should().Be(0);
            result.Console.AllText().Should().Be(@"lala
");
        }

        class App
        {
            public void Ansi(IAnsiConsole ansiConsole, string text)
            {
                ansiConsole.WriteLine(text);
            }

            public void Piped(IConsole console, string[] texts)
            {
                console.WriteLine(texts.ToCsv(Environment.NewLine));
            }
        }
    }
}